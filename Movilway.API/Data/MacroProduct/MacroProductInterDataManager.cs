using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Objects.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using Movilway.API.Core;
using Movilway.API.Service.ExtendedApi.DataContract;
using Movilway.API.Service.ExtendedApi.DataContract.MacroInternational;
using System.Diagnostics;

namespace Movilway.API.Data.MacroProduct
{
    public class MacroProductInterDataManager
    {
        private MacroProductosEntities _db;

        public GetMacroProductInternationalResponse GetMacroProductsByCategoryInter(int countryId, ProductList platformProducts, int deviceType)
        {
            _db = new MacroProductosEntities();
            var result = new GetMacroProductInternationalResponse() { Countries = new List<IntCountry>() };

            try
            {
                _db.Database.Connection.Open();

                //Se obtienen los datos con los que se formara el arbol base: Pais => Categoria => Subcategoria
                var Countries = (from co in _db.MacroProduct
                                 where co.CountryId == (countryId == 0 ? co.CountryId : countryId)
                                 select new
                                     {
                                         CountryId = co.CountryId,
                                         CountryName = String.Empty
                                     }).Distinct().ToList();

                var Categories = (from c in _db.Category
                                  orderby c.Rank
                                  select new
                                  {
                                      CategoryId = c.CategoryId,
                                      CategoryName = c.CategoryName,
                                      CategoryDescription = c.Description,
                                      Subcategories = (from sub in _db.SubCategory
                                                       where sub.CategoryId == c.CategoryId
                                                       orderby sub.Rank
                                                       select new IntSubCategory()
                                                       {
                                                           SubcategoryId = sub.SubCategoryId,
                                                           SubcategoryName = sub.SubCategoryName,
                                                           SubcategoryDescription = sub.Description
                                                       })
                                  }).ToList();

                var mProducts = (from mp in _db.MacroProduct
                                 join sCat in _db.SubCategory on mp.SubcategoryId equals sCat.SubCategoryId
                                 join amt in _db.AmountType on mp.AmountTypeId equals amt.AmountTypeId
                                 where mp.CountryId == (countryId == 0 ? mp.CountryId : countryId)
                                  && !mp.AccessType.Any(p => p.AccessTypeId == deviceType)
                                 orderby sCat.Rank, mp.Rank
                                 select new
                                 {
                                     CountryId = mp.CountryId,
                                     CategoryId = sCat.CategoryId,
                                     SCategoryId = sCat.SubCategoryId,
                                     MacroProducDetails = new IntMacroProduct()
                                     {
                                         MacroProductId = mp.MacroProductId,
                                         MacroProductName = mp.MacroProductName,
                                         PercentageFee = mp.FeePercent,
                                         FlatFee = mp.FeePlus,
                                         RePrint = mp.Reprint,
                                         AmountType = amt.AmountTypeName,
                                         Description = mp.Description,
                                         MandatoryPrint = mp.MandatoryPrint,
                                         BalanceType = mp.BalanceType.BalanceTypeName,
                                         Rank = mp.Rank,
                                         OwnerCountryId = (mp.OwnerCountryId == null ? 0 : (int)mp.OwnerCountryId),
                                         CurrencyPurchase = mp.CurrencyPurchase,
                                         CurrencySale = mp.CurrencySale,
                                         ExchangeRate = (mp.ExchangeRate == null ? 0 :  (Decimal)mp.ExchangeRate),
                                         AmountField = mp.AmountField
                                     },
                                     FieldList = (from fld in _db.MacroProductField
                                                  where fld.MacroProductId == mp.MacroProductId && fld.CountryId == mp.CountryId
                                                  orderby fld.MacroProductFieldId
                                                  select new
                                                  {
                                                      Id = fld.MacroProductFieldId,
                                                      MacroProductId = mp.MacroProductId,
                                                      CountryId = mp.CountryId,
                                                      Name = fld.FieldName,
                                                      Type = fld.FieldType.FieldTypeName,
                                                      Length = fld.FieldLength,
                                                      Format = fld.FieldFormat,
                                                      Description = fld.Description,
                                                      Mandatory = fld.Mandatory,
                                                      MapValueNameTo = fld.MapValueNameTo,
                                                      MapValueTo = fld.MapValueTo,
                                                      Min = fld.Min,
                                                      Max = fld.Max,
                                                      Values = _db.MacroProductValue.Where(v => v.MacroProductId == fld.MacroProductId && v.CountryId == fld.CountryId && v.MacroProductFieldId == fld.MacroProductFieldId)
                                                                                          .OrderBy(x => x.MacroProductValueId).Select(x => new Service.ExtendedApi.DataContract.MacroInternational.IntFieldValues()
                                                                                          {
                                                                                              ValueName = x.ValueName,
                                                                                              Value = x.Value
                                                                                          })
                                                  }),
                                     Channels = (from ch in _db.MacroProductChannel
                                                 where ch.MacroProductId == mp.MacroProductId && ch.CountryId == mp.CountryId
                                                 select new IntChannel()
                                                 {
                                                     MacroProductChannelId = ch.MacroProductChannelId,
                                                     Name = String.Empty,
                                                     Load = ch.Load
                                                 })
                                 }).ToList();

                //Creamos el objeto de respuesta.

                //Se agregan los paises y cada uno se le crea el arbol de categorias -> subcategorias.
                foreach (var cou in Countries.AsParallel())
                {
                    var country = new IntCountry();
                    country.CountryId = cou.CountryId;
                    country.CountryName = cou.CountryName;

                    foreach (var cat in Categories.AsParallel())
                    {
                        country.Categories.Add(new IntCategory()
                        {
                            CategoryId = cat.CategoryId,
                            CategoryName = cat.CategoryName,
                            CategoryDescription = cat.CategoryDescription,
                            Subcategories = cat.Subcategories.ToList()
                        });
                    }

                    result.Countries.Add(country);
                }

                foreach (var cou in result.Countries.AsParallel())
                {
                    foreach (var cat in cou.Categories.AsParallel())
                    {
                        foreach (var scat in cat.Subcategories.AsParallel())
                        {
                            //Se buscan todos los macroproductos que corresponden al pais -> categoria -> subcategoria
                            var mproducts = (from mp in mProducts.AsParallel()
                                             where mp.CountryId == cou.CountryId && mp.CategoryId == cat.CategoryId && mp.SCategoryId == scat.SubcategoryId
                                             select mp).ToList();

                            if (mproducts.Any())
                            {
                                foreach (var data in mproducts.AsParallel())
                                {
                                    var mp = new IntMacroProduct()
                                    {
                                        MacroProductId = data.MacroProducDetails.MacroProductId,
                                        MacroProductName = data.MacroProducDetails.MacroProductName.RemoveAccents(),
                                        PercentageFee = data.MacroProducDetails.PercentageFee,
                                        FlatFee = data.MacroProducDetails.FlatFee,
                                        RePrint = data.MacroProducDetails.RePrint,
                                        AmountType = data.MacroProducDetails.AmountType,
                                        BalanceType = data.MacroProducDetails.BalanceType,
                                        Fields = new List<IntField>(),
                                        Channels = new List<IntChannel>(),
                                        Rank = data.MacroProducDetails.Rank,
                                        OwnerCountryId = data.MacroProducDetails.OwnerCountryId,
                                        CurrencyPurchase = data.MacroProducDetails.CurrencyPurchase,
                                        CurrencySale = data.MacroProducDetails.CurrencySale,
                                        ExchangeRate = data.MacroProducDetails.ExchangeRate,
                                        AmountField = data.MacroProducDetails.AmountField
                                    };

                                    var channels = data.Channels.ToList();

                                    //Se agrega el nombre del Channel.
                                    foreach (var ch in channels.AsParallel())
                                    {
                                        ch.Name = platformProducts.AsParallel().FirstOrDefault(pp => pp.Key == ch.MacroProductChannelId.ToString()).Value;
                                    }

                                    //Lista los productos que estan asignados al cliente.
                                    var acceptedProducts = platformProducts.AsParallel().Select(p => Convert.ToInt32(p.Key)).ToList();

                                    //Elimina los channels que no estan permitidos para el cliente.
                                    channels.RemoveAll(ch => !acceptedProducts.Contains(ch.MacroProductChannelId));

                                    if (channels.Count > 0)
                                    {
                                        //Agrega los Fields del macroproducto.
                                        foreach (var x in data.FieldList)
                                        {
                                            mp.Fields.Add(new IntField()
                                                {
                                                    Id = x.Id,
                                                    MacroProductId = x.MacroProductId,
                                                    CountryId = x.CountryId,
                                                    Name = x.Name,
                                                    Type = x.Type,
                                                    Length = x.Length,
                                                    Format = x.Format,
                                                    Description = x.Description,
                                                    Mandatory = x.Mandatory,
                                                    MapValueNameTo = x.MapValueNameTo,
                                                    MapValueTo = x.MapValueTo,
                                                    Min = x.Min,
                                                    Max = x.Max,
                                                    Values = x.Values.ToList()
                                                });
                                        }

                                        //Agrega los Channels del macroproducto.
                                        mp.Channels.AddRange(channels.ToList());

                                        scat.MacroProducts.Add(mp);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                _db.Database.Connection.Close();
            }

            result.Rank = String.Join(";", result.Countries.SelectMany(c => c.Categories).SelectMany(c => c.Subcategories).SelectMany(s => s.MacroProducts).OrderBy(m => m.Rank).Select(m => m.MacroProductId).Distinct());

            return result;
        }

        public GetMacroProductInternationalResponse GetMacroProductsInter(int countryId, ProductList platformProducts, int deviceType)
        {
            _db = new MacroProductosEntities();
            var result = new GetMacroProductInternationalResponse() { MacroProducts = new List<IntMacroProduct>() };

            try
            {
                _db.Database.Connection.Open();

                var mProducts = (from mp in _db.MacroProduct
                                 join sCat in _db.SubCategory on mp.SubcategoryId equals sCat.SubCategoryId
                                 join amt in _db.AmountType on mp.AmountTypeId equals amt.AmountTypeId
                                 where mp.CountryId == (countryId == 0 ? mp.CountryId : countryId)
                                 && !mp.AccessType.Any(p => p.AccessTypeId == deviceType)  
                                 orderby sCat.Rank, mp.Rank
                                 select new
                                 {
                                     CountryId = mp.CountryId,
                                     CategoryId = sCat.CategoryId,
                                     SCategoryId = sCat.SubCategoryId,
                                     MacroProducDetails = new IntMacroProduct()
                                     {
                                         MacroProductId = mp.MacroProductId,
                                         MacroProductName = mp.MacroProductName,
                                         CountryId = mp.CountryId,
                                         PercentageFee = mp.FeePercent,
                                         FlatFee = mp.FeePlus,
                                         RePrint = mp.Reprint,
                                         AmountType = amt.AmountTypeName,
                                         Description = mp.Description,
                                         MandatoryPrint = mp.MandatoryPrint,
                                         BalanceType = mp.BalanceType.BalanceTypeName,
                                         Rank = mp.Rank,
                                         OwnerCountryId = (mp.OwnerCountryId == null ? 0 : (int)mp.OwnerCountryId),
                                         CurrencyPurchase = mp.CurrencyPurchase,
                                         CurrencySale = mp.CurrencySale,
                                         ExchangeRate = (mp.ExchangeRate == null ? 0 : (Decimal)mp.ExchangeRate),
                                         AmountField = mp.AmountField
                                     },
                                     FieldList = (from fld in _db.MacroProductField
                                                  where fld.MacroProductId == mp.MacroProductId && fld.CountryId == mp.CountryId
                                                  orderby fld.MacroProductFieldId
                                                  select new
                                                  {
                                                      Id = fld.MacroProductFieldId,
                                                      MacroProductId = mp.MacroProductId,
                                                      CountryId = mp.CountryId,
                                                      Name = fld.FieldName,
                                                      Type = fld.FieldType.FieldTypeName,
                                                      Length = fld.FieldLength,
                                                      Format = fld.FieldFormat,
                                                      Description = fld.Description,
                                                      Mandatory = fld.Mandatory,
                                                      MapValueNameTo = fld.MapValueNameTo,
                                                      MapValueTo = fld.MapValueTo,
                                                      Min = fld.Min,
                                                      Max = fld.Max,
                                                      Values = _db.MacroProductValue.Where(v => v.MacroProductId == fld.MacroProductId && v.CountryId == fld.CountryId && v.MacroProductFieldId == fld.MacroProductFieldId)
                                                                                          .OrderBy(x => x.MacroProductValueId).Select(x => new Service.ExtendedApi.DataContract.MacroInternational.IntFieldValues()
                                                                                          {
                                                                                              ValueName = x.ValueName,
                                                                                              Value = x.Value
                                                                                          })
                                                  }),
                                     Channels = (from ch in _db.MacroProductChannel
                                                 where ch.MacroProductId == mp.MacroProductId && ch.CountryId == mp.CountryId
                                                 select new IntChannel()
                                                 {
                                                     MacroProductChannelId = ch.MacroProductChannelId,
                                                     Name = String.Empty,
                                                     Load = ch.Load
                                                 })
                                 }).ToList();

                //Creamos el objeto de respuesta.

                foreach (var data in mProducts.AsParallel())
                {
                    var mp = new IntMacroProduct()
                    {
                        MacroProductId = data.MacroProducDetails.MacroProductId,
                        MacroProductName = data.MacroProducDetails.MacroProductName.RemoveAccents(),
                        CountryId = data.MacroProducDetails.CountryId,
                        PercentageFee = data.MacroProducDetails.PercentageFee,
                        FlatFee = data.MacroProducDetails.FlatFee,
                        RePrint = data.MacroProducDetails.RePrint,
                        AmountType = data.MacroProducDetails.AmountType,
                        BalanceType = data.MacroProducDetails.BalanceType,
                        Fields = new List<IntField>(),
                        Channels = new List<IntChannel>(),
                        Rank = data.MacroProducDetails.Rank,
                        OwnerCountryId = data.MacroProducDetails.OwnerCountryId,
                        CurrencyPurchase = data.MacroProducDetails.CurrencyPurchase,
                        CurrencySale = data.MacroProducDetails.CurrencySale,
                        ExchangeRate = data.MacroProducDetails.ExchangeRate,
                        AmountField = data.MacroProducDetails.AmountField
                    };

                    var channels = data.Channels.ToList();

                    //Se agrega el nombre del Channel.
                    foreach (var ch in channels.AsParallel())
                    {
                        ch.Name = platformProducts.AsParallel().FirstOrDefault(pp => pp.Key == ch.MacroProductChannelId.ToString()).Value;
                    }

                    //Lista los productos que estan asignados al cliente.
                    var acceptedProducts = platformProducts.AsParallel().Select(p => Convert.ToInt32(p.Key)).ToList();

                    //Elimina los channels que no estan permitidos para el cliente.
                    channels.RemoveAll(ch => !acceptedProducts.Contains(ch.MacroProductChannelId));

                    if (channels.Count > 0)
                    {
                        //Agrega los Fields del macroproducto.
                        foreach (var x in data.FieldList)
                        {
                            mp.Fields.Add(new IntField()
                            {
                                Id = x.Id,
                                MacroProductId = x.MacroProductId,
                                CountryId = x.CountryId,
                                Name = x.Name,
                                Type = x.Type,
                                Length = x.Length,
                                Format = x.Format,
                                Description = x.Description,
                                Mandatory = x.Mandatory,
                                MapValueNameTo = x.MapValueNameTo,
                                MapValueTo = x.MapValueTo,
                                Min = x.Min,
                                Max = x.Max,
                                Values = x.Values.ToList()
                            });
                        }

                        //Agrega los Channels del macroproducto.
                        mp.Channels.AddRange(channels.ToList());

                        result.MacroProducts.Add(mp);
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                _db.Database.Connection.Close();
            }

            return result;
        }

        

    }
}