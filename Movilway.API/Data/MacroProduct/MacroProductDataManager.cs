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
using Movilway.API.Service.ExtendedApi.DataContract.Macro;
using System.Diagnostics;

namespace Movilway.API.Data.MacroProduct
{
    public class MacroProductDataManager
    {

        private MacroProductosEntities _db;

        public Service.ExtendedApi.DataContract.Macro.GetMacroProductListByCategoryResponse GetMacroProductsByCategory(int platformId, int countryId, ProductList platformProducts, int deviceType)
        {
            try
            {
                _db = new MacroProductosEntities();
                var result = new Service.ExtendedApi.DataContract.Macro.GetMacroProductListByCategoryResponse() { Categories = new List<Service.ExtendedApi.DataContract.Macro.Category>() };

                
                //Valor para incluir los montos correspondientes al minimo y maximo de los montos abiertos.
                var includeMinMax = ConfigurationManager.AppSettings["MP_IncludeMinMax"] ?? String.Empty;
                var RequireQueryDeviceTypes = (ConfigurationManager.AppSettings["MP_RequireQueryDeviceTypes"] ?? String.Empty).Split(',');
                var _setRequireQuery = RequireQueryDeviceTypes.Contains(deviceType.ToString());
                
                _db.Database.Connection.Open();
                var categories = (from c in _db.Category
                                  where c.StatusId == "AC" && (c.CountryId == countryId || c.CountryId == 0)
                                  orderby c.Rank
                                  select new
                                  {
                                      c.CategoryId,
                                      c.CategoryName,
                                      c.Description,
                                      Subcategories = (from sub in _db.SubCategory
                                                       where sub.CategoryId == c.CategoryId && sub.StatusId == "AC"
                                                       orderby sub.Rank
                                                       select new Service.ExtendedApi.DataContract.Macro.SubCategory()
                                                       {
                                                           SubcategoryId = sub.SubCategoryId,
                                                           SubcategoryName = sub.SubCategoryName,
                                                           SubcategoryDescription = sub.Description
                                                       })
                                  }).ToList();
                
                var mProducts = (from mp in _db.MacroProduct
                                 join sCat in _db.SubCategory on mp.SubcategoryId equals sCat.SubCategoryId
                                 join amt in _db.AmountType on mp.AmountTypeId equals amt.AmountTypeId
                                 where (mp.CountryId == countryId || mp.CountryId == 0) && mp.StatusId == "AC"
                                 && !mp.AccessType.Any(p => p.AccessTypeId == deviceType)
                                 orderby sCat.Rank, mp.Rank
                                 select new
                                 {
                                     SCategory = sCat.SubCategoryId,
                                     MacroProducDetails = new Service.ExtendedApi.DataContract.Macro.MacroProduct()
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
                                         RequireQuery = mp.RequireQuery
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
                                                                                               .OrderBy(x => x.MacroProductValueId).Select(x => new Service.ExtendedApi.DataContract.Macro.FieldValues()
                                                                                               {
                                                                                                   ValueName = x.ValueName,
                                                                                                   Value = x.Value
                                                                                               })
                                                  }),
                                     Channels = (from ch in _db.MacroProductChannel
                                                 where ch.MacroProductId == mp.MacroProductId && ch.CountryId == mp.CountryId
                                                 select new Service.ExtendedApi.DataContract.Macro.Channel()
                                                 {
                                                     MacroProductChannelId = ch.MacroProductChannelId,
                                                     Name = String.Empty,
                                                     Load = ch.Load
                                                 })
                                 }).ToList().GroupBy(x => x.SCategory, (sCatId, macroProds) => new
                                 {
                                     MpSubCategory = sCatId,
                                     MpData = macroProds.ToList()
                                 });

                //Creamos el objeto de respuesta.

                //Categorias:
                foreach (var cat in categories)
                {
                    result.Categories.Add(new Service.ExtendedApi.DataContract.Macro.Category()
                    {
                        CategoryId = cat.CategoryId,
                        CategoryName = cat.CategoryName,
                        CategoryDescription = cat.Description,
                        Subcategories = cat.Subcategories.ToList()
                    });
                }

                //Subcategorias:
                foreach (var cat in result.Categories)
                {
                    foreach (var subcategory in cat.Subcategories)
                    {
                        //Los macroproductos estan agrupados por subcategoria.
                        foreach (var group in mProducts)
                        {
                            //Si es la subcategoria del grupo, se agregan sus macroproductos.
                            if (subcategory.SubcategoryId != group.MpSubCategory) continue;

                            foreach (var data in group.MpData)
                            {
                                var mp = new Service.ExtendedApi.DataContract.Macro.MacroProduct()
                                {
                                    MacroProductId = data.MacroProducDetails.MacroProductId,
                                    MacroProductName = data.MacroProducDetails.MacroProductName.RemoveAccents(),
                                    PercentageFee = data.MacroProducDetails.PercentageFee,
                                    FlatFee = data.MacroProducDetails.FlatFee,
                                    RePrint = data.MacroProducDetails.RePrint,
                                    AmountType = data.MacroProducDetails.AmountType,
                                    BalanceType = data.MacroProducDetails.BalanceType,
                                    Fields = new List<Service.ExtendedApi.DataContract.Macro.Field>(),
                                    Channels = new List<Service.ExtendedApi.DataContract.Macro.Channel>(),
                                    Rank = data.MacroProducDetails.Rank,
                                    Description = data.MacroProducDetails.Description,
                                    RequireQuery = _setRequireQuery ? data.MacroProducDetails.RequireQuery : null
                                    //Amounts = new List<Service.ExtendedApi.DataContract.Macro.AmountEntity>(),
                                };

                                var channels = data.Channels.ToList();

                                //Se agrega el nombre del Channel.
                                foreach (var ch in channels)
                                {
                                    ch.Name = platformProducts.FirstOrDefault(pp => pp.Key == ch.MacroProductChannelId.ToString()).Value;
                                }

                                //Lista los productos que estan asignados al cliente.
                                var acceptedProducts = platformProducts.Select(p => Convert.ToInt32(p.Key)).ToList();

                                //Elimina los channels que no estan permitidos para el cliente.
                                channels.RemoveAll(ch => !acceptedProducts.Contains(ch.MacroProductChannelId));

                                if (channels.Count > 0)
                                {
                                    //var amounts = data.Amounts.ToList().RemoveAccents();
                                    var fields = data.FieldList.ToList();

                                    //Agrega los Fields del macroproducto.
                                    foreach (var x in data.FieldList)
                                    {
                                        mp.Fields.Add(new Field()
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
                                    mp.Channels.AddRange(channels.ToList().RemoveAccents());

                                    subcategory.MacroProducts.Add(mp);
                                }
                            }
                        }
                    }
                }

                var catsToEliminate = new List<Service.ExtendedApi.DataContract.Macro.Category>();
                foreach (var item in result.Categories)
                    if (item.Subcategories.SelectMany(sc => sc.MacroProducts).Count() == 0)
                        catsToEliminate.Add(item);

                foreach (var item in catsToEliminate)
                    result.Categories.Remove(item);

                result.Rank = String.Join(";", result.Categories.SelectMany(c => c.Subcategories).SelectMany(s => s.MacroProducts).OrderBy(m => m.Rank).Select(m => m.MacroProductId));
                
                return result;
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                _db.Database.Connection.Close();
            }

        }

        public Service.ExtendedApi.DataContract.Macro.GetMacroProductListByCategoryLightResponse GetMacroProductsByCategoryLight(int platformId, int countryId, ProductList platformProducts, int deviceType)
        {
            _db = new MacroProductosEntities();
            var result = new Service.ExtendedApi.DataContract.Macro.GetMacroProductListByCategoryLightResponse() { Categories = new List<Service.ExtendedApi.DataContract.Macro.CategoryLight>() };

            //Valor para incluir los montos correspondientes al minimo y maximo de los montos abiertos.
            //var includeMinMax = ConfigurationManager.AppSettings["MP_IncludeMinMax"] ?? String.Empty;
            var RequireQueryDeviceTypes = (ConfigurationManager.AppSettings["MP_RequireQueryDeviceTypes"] ?? String.Empty).Split(',');
            var _setRequireQuery = RequireQueryDeviceTypes.Contains(deviceType.ToString());
            try
            {
                _db.Database.Connection.Open();

                var categories = (from c in _db.Category
                                  where c.StatusId == "AC" && (c.CountryId == countryId || c.CountryId == 0)
                                  orderby c.Rank
                                  select new
                                  {
                                      c.CategoryId,
                                      c.CategoryName,
                                      c.Description,
                                      Subcategories = (from sub in _db.SubCategory
                                                       where sub.CategoryId == c.CategoryId && sub.StatusId == "AC"
                                                       orderby sub.Rank
                                                       select new Service.ExtendedApi.DataContract.Macro.SubCategory()
                                                       {
                                                           SubcategoryId = sub.SubCategoryId,
                                                           SubcategoryName = sub.SubCategoryName,
                                                           SubcategoryDescription = sub.Description
                                                       })
                                  }).ToList();




                var mProducts = (from mp in _db.MacroProduct
                                 join sCat in _db.SubCategory on mp.SubcategoryId equals sCat.SubCategoryId
                                 join amt in _db.AmountType on mp.AmountTypeId equals amt.AmountTypeId
                                 where (mp.CountryId == countryId || mp.CountryId == 0) && mp.StatusId == "AC"
                                 && !mp.AccessType.Any(p => p.AccessTypeId == deviceType)
                                 orderby sCat.Rank, mp.Rank
                                 select new
                                 {
                                     SCategory = sCat.SubCategoryId,
                                     MacroProducDetails = new Service.ExtendedApi.DataContract.Macro.MacroProduct()
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
                                         RequireQuery = mp.RequireQuery
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
                                                                                               .OrderBy(x => x.MacroProductValueId).Select(x => new Service.ExtendedApi.DataContract.Macro.FieldValues()
                                                                                               {
                                                                                                   ValueName = x.ValueName,
                                                                                                   Value = x.Value
                                                                                               })
                                                  }),
                                     Channels = (from ch in _db.MacroProductChannel
                                                 where ch.MacroProductId == mp.MacroProductId && ch.CountryId == mp.CountryId
                                                 select new Service.ExtendedApi.DataContract.Macro.Channel()
                                                 {
                                                     MacroProductChannelId = ch.MacroProductChannelId,
                                                     Name = String.Empty,
                                                     Load = ch.Load
                                                 })
                                 }).ToList().GroupBy(x => x.SCategory, (sCatId, macroProds) => new
                                 {
                                     MpSubCategory = sCatId,
                                     MpData = macroProds.ToList()
                                 });

                //Creamos el objeto de respuesta.

                //Categorias:
                foreach (var cat in categories)
                {
                    var newCat = new Service.ExtendedApi.DataContract.Macro.CategoryLight()
                    {
                        CategoryId = cat.CategoryId,
                        CategoryName = cat.CategoryName,
                        CategoryDescription = cat.Description,
                        Subcategories = new List<SubCategoryLight>()
                    };

                    foreach (var subcat in cat.Subcategories)
                    {
                        var newSubCat = new SubCategoryLight()
                        {
                            SubcategoryId = subcat.SubcategoryId,
                            SubcategoryName = subcat.SubcategoryName,
                            SubcategoryDescription = subcat.SubcategoryDescription
                        };

                        //foreach (var macrop in subcat.MacroProducts)
                        //{
                        //    newSubCat.MacroProducts.Add(new MacroProductLight()
                        //    {
                        //        MacroProductId = macrop.MacroProductId,
                        //        MacroProductName = macrop.MacroProductName,
                        //        RequireQuery = macrop.RequireQuery,
                        //        Rank = macrop.Rank
                        //    });
                        //}

                        //if (newSubCat.MacroProducts.Count > 0)
                        newCat.Subcategories.Add(newSubCat);
                    }

                    if (newCat.Subcategories != null)
                        result.Categories.Add(newCat);
                }

                //Subcategorias:
                foreach (var cat in result.Categories)
                {
                    foreach (var subcategory in cat.Subcategories)
                    {
                        //Los macroproductos estan agrupados por subcategoria.
                        foreach (var group in mProducts)
                        {
                            //Si es la subcategoria del grupo, se agregan sus macroproductos.
                            if (subcategory.SubcategoryId != group.MpSubCategory) continue;

                            foreach (var data in group.MpData)
                            {
                                var mp = new Service.ExtendedApi.DataContract.Macro.MacroProductLight()
                                {
                                    MacroProductId = data.MacroProducDetails.MacroProductId,
                                    MacroProductName = data.MacroProducDetails.MacroProductName.RemoveAccents(),
                                    //PercentageFee = data.MacroProducDetails.PercentageFee,
                                    //FlatFee = data.MacroProducDetails.FlatFee,
                                    //RePrint = data.MacroProducDetails.RePrint,
                                    //AmountType = data.MacroProducDetails.AmountType,
                                    //BalanceType = data.MacroProducDetails.BalanceType,
                                    //Fields = new List<Service.ExtendedApi.DataContract.Macro.Field>(),
                                    //Channels = new List<Service.ExtendedApi.DataContract.Macro.Channel>(),
                                    Rank = data.MacroProducDetails.Rank,
                                    //Description = data.MacroProducDetails.Description,
                                    RequireQuery = _setRequireQuery ? data.MacroProducDetails.RequireQuery : null
                                    //Amounts = new List<Service.ExtendedApi.DataContract.Macro.AmountEntity>(),
                                };

                                var channels = data.Channels.ToList();

                                //Se agrega el nombre del Channel.
                                foreach (var ch in channels)
                                {
                                    ch.Name = platformProducts.FirstOrDefault(pp => pp.Key == ch.MacroProductChannelId.ToString()).Value;
                                }

                                //Lista los productos que estan asignados al cliente.
                                var acceptedProducts = platformProducts.Select(p => Convert.ToInt32(p.Key)).ToList();

                                //Elimina los channels que no estan permitidos para el cliente.
                                channels.RemoveAll(ch => !acceptedProducts.Contains(ch.MacroProductChannelId));

                                if (channels.Count > 0)
                                {
                                    //var amounts = data.Amounts.ToList().RemoveAccents();
                                    //var fields = data.FieldList.ToList();

                                    ////Agrega los Fields del macroproducto.
                                    //foreach (var x in data.FieldList)
                                    //{
                                    //    mp.Fields.Add(new Field()
                                    //    {
                                    //        Id = x.Id,
                                    //        MacroProductId = x.MacroProductId,
                                    //        CountryId = x.CountryId,
                                    //        Name = x.Name,
                                    //        Type = x.Type,
                                    //        Length = x.Length,
                                    //        Format = x.Format,
                                    //        Description = x.Description,
                                    //        Mandatory = x.Mandatory,
                                    //        MapValueNameTo = x.MapValueNameTo,
                                    //        MapValueTo = x.MapValueTo,
                                    //        Min = x.Min,
                                    //        Max = x.Max,
                                    //        Values = x.Values.ToList()
                                    //    });
                                    //}

                                    ////Agrega los Channels del macroproducto.
                                    //mp.Channels.AddRange(channels.ToList().RemoveAccents());

                                    subcategory.MacroProducts.Add(mp);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                _db.Database.Connection.Close();
            }

            var catsToEliminate = new List<Service.ExtendedApi.DataContract.Macro.CategoryLight>();
            foreach (var item in result.Categories)
                if (item.Subcategories.SelectMany(sc => sc.MacroProducts).Count() == 0)
                    catsToEliminate.Add(item);

            foreach (var item in catsToEliminate)
                result.Categories.Remove(item);

            //result.Categories.SelectMany(c => c.Subcategories).Where(sc => sc.MacroProducts.Count == 0).Select();

            result.Rank = String.Join(";", result.Categories.SelectMany(c => c.Subcategories).SelectMany(s => s.MacroProducts).OrderBy(m => m.Rank).Select(m => m.MacroProductId));

            return result;
        }

        public Service.ExtendedApi.DataContract.Macro.GetMacroProductDetailsResponse GetMacroProductDetails(int platformId, int countryId, int macroProductId, ProductList platformProducts, int deviceType)
        {
            _db = new MacroProductosEntities();
            var result = new Service.ExtendedApi.DataContract.Macro.GetMacroProductDetailsResponse() { Categories = new List<Service.ExtendedApi.DataContract.Macro.Category>() };

            //Valor para incluir los montos correspondientes al minimo y maximo de los montos abiertos.
            //var includeMinMax = ConfigurationManager.AppSettings["MP_IncludeMinMax"] ?? String.Empty;
            var RequireQueryDeviceTypes = (ConfigurationManager.AppSettings["MP_RequireQueryDeviceTypes"] ?? String.Empty).Split(',');
            var _setRequireQuery = RequireQueryDeviceTypes.Contains(deviceType.ToString());
            try
            {
                _db.Database.Connection.Open();

                var categories = (from c in _db.Category
                                  where c.StatusId == "AC" && (c.CountryId == countryId || c.CountryId == 0)
                                  orderby c.Rank
                                  select new
                                  {
                                      c.CategoryId,
                                      c.CategoryName,
                                      c.Description,
                                      Subcategories = (from sub in _db.SubCategory
                                                       where sub.CategoryId == c.CategoryId && sub.StatusId == "AC"
                                                       orderby sub.Rank
                                                       select new Service.ExtendedApi.DataContract.Macro.SubCategory()
                                                       {
                                                           SubcategoryId = sub.SubCategoryId,
                                                           SubcategoryName = sub.SubCategoryName,
                                                           SubcategoryDescription = sub.Description
                                                       })
                                  }).ToList();




                var mProducts = (from mp in _db.MacroProduct
                                 join sCat in _db.SubCategory on mp.SubcategoryId equals sCat.SubCategoryId
                                 join amt in _db.AmountType on mp.AmountTypeId equals amt.AmountTypeId
                                 where (mp.CountryId == countryId || mp.CountryId == 0) && mp.MacroProductId == macroProductId && mp.StatusId == "AC"
                                 && !mp.AccessType.Any(p => p.AccessTypeId == deviceType)
                                 orderby sCat.Rank, mp.Rank
                                 select new
                                 {
                                     SCategory = sCat.SubCategoryId,
                                     MacroProducDetails = new Service.ExtendedApi.DataContract.Macro.MacroProduct()
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
                                         RequireQuery = mp.RequireQuery
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
                                                                                               .OrderBy(x => x.MacroProductValueId).Select(x => new Service.ExtendedApi.DataContract.Macro.FieldValues()
                                                                                               {
                                                                                                   ValueName = x.ValueName,
                                                                                                   Value = x.Value
                                                                                               })
                                                  }),
                                     Channels = (from ch in _db.MacroProductChannel
                                                 where ch.MacroProductId == mp.MacroProductId && ch.CountryId == mp.CountryId
                                                 select new Service.ExtendedApi.DataContract.Macro.Channel()
                                                 {
                                                     MacroProductChannelId = ch.MacroProductChannelId,
                                                     Name = String.Empty,
                                                     Load = ch.Load
                                                 })
                                 }).ToList().GroupBy(x => x.SCategory, (sCatId, macroProds) => new
                                 {
                                     MpSubCategory = sCatId,
                                     MpData = macroProds.ToList()
                                 });

                //Creamos el objeto de respuesta.

                //Categorias:
                foreach (var cat in categories)
                {
                    result.Categories.Add(new Service.ExtendedApi.DataContract.Macro.Category()
                    {
                        CategoryId = cat.CategoryId,
                        CategoryName = cat.CategoryName,
                        CategoryDescription = cat.Description,
                        Subcategories = cat.Subcategories.ToList()
                    });
                }

                //Subcategorias:
                foreach (var cat in result.Categories)
                {
                    foreach (var subcategory in cat.Subcategories)
                    {
                        //Los macroproductos estan agrupados por subcategoria.
                        foreach (var group in mProducts)
                        {
                            //Si es la subcategoria del grupo, se agregan sus macroproductos.
                            if (subcategory.SubcategoryId != group.MpSubCategory) continue;

                            foreach (var data in group.MpData)
                            {
                                var mp = new Service.ExtendedApi.DataContract.Macro.MacroProduct()
                                {
                                    MacroProductId = data.MacroProducDetails.MacroProductId,
                                    MacroProductName = data.MacroProducDetails.MacroProductName.RemoveAccents(),
                                    PercentageFee = data.MacroProducDetails.PercentageFee,
                                    FlatFee = data.MacroProducDetails.FlatFee,
                                    RePrint = data.MacroProducDetails.RePrint,
                                    AmountType = data.MacroProducDetails.AmountType,
                                    BalanceType = data.MacroProducDetails.BalanceType,
                                    Fields = new List<Service.ExtendedApi.DataContract.Macro.Field>(),
                                    Channels = new List<Service.ExtendedApi.DataContract.Macro.Channel>(),
                                    Rank = data.MacroProducDetails.Rank,
                                    Description = data.MacroProducDetails.Description,
                                    RequireQuery = _setRequireQuery ? data.MacroProducDetails.RequireQuery : null
                                    //Amounts = new List<Service.ExtendedApi.DataContract.Macro.AmountEntity>(),
                                };

                                var channels = data.Channels.ToList();

                                //Se agrega el nombre del Channel.
                                foreach (var ch in channels)
                                {
                                    ch.Name = platformProducts.FirstOrDefault(pp => pp.Key == ch.MacroProductChannelId.ToString()).Value;
                                }

                                //Lista los productos que estan asignados al cliente.
                                var acceptedProducts = platformProducts.Select(p => Convert.ToInt32(p.Key)).ToList();

                                //Elimina los channels que no estan permitidos para el cliente.
                                channels.RemoveAll(ch => !acceptedProducts.Contains(ch.MacroProductChannelId));

                                if (channels.Count > 0)
                                {
                                    //var amounts = data.Amounts.ToList().RemoveAccents();
                                    var fields = data.FieldList.ToList();

                                    //Agrega los Fields del macroproducto.
                                    foreach (var x in data.FieldList)
                                    {
                                        mp.Fields.Add(new Field()
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
                                    mp.Channels.AddRange(channels.ToList().RemoveAccents());

                                    subcategory.MacroProducts.Add(mp);
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

            var catsToEliminate = new List<Service.ExtendedApi.DataContract.Macro.Category>();
            foreach (var item in result.Categories)
                if (item.Subcategories.SelectMany(sc => sc.MacroProducts).Count() == 0)
                    catsToEliminate.Add(item);

            foreach (var item in catsToEliminate)
                result.Categories.Remove(item);

            var subCatsToEliminate = new List<Service.ExtendedApi.DataContract.Macro.SubCategory>();
            foreach (var item in result.Categories)
                foreach (var item2 in item.Subcategories)
                    if (item2.MacroProducts.Count() == 0)
                        subCatsToEliminate.Add(item2);

            foreach (var item in subCatsToEliminate)
                result.Categories[0].Subcategories.Remove(item);

            //result.Categories.SelectMany(c => c.Subcategories).Where(sc => sc.MacroProducts.Count == 0).Select();

            //result.Rank = String.Join(";", result.Categories.SelectMany(c => c.Subcategories).SelectMany(s => s.MacroProducts).OrderBy(m => m.Rank).Select(m => m.MacroProductId));

            return result;
        }

        public Service.ExtendedApi.DataContract.Macro.GetMacroProductListResponse GetMacroProducts(int platformId, int countryId, ProductList platformProducts, int deviceType)
        {
            _db = new MacroProductosEntities();
            var result = new Service.ExtendedApi.DataContract.Macro.GetMacroProductListResponse() { MacroProducts = new List<Service.ExtendedApi.DataContract.Macro.MacroProduct>() };

            try
            {

                var RequireQueryDeviceTypes = (ConfigurationManager.AppSettings["MP_RequireQueryDeviceTypes"] ?? String.Empty).Split(',');
                var _setRequireQuery = RequireQueryDeviceTypes.Contains(deviceType.ToString());

                _db.Database.Connection.Open();

                var mProducts = (from mp in _db.MacroProduct
                                 join amt in _db.AmountType on mp.AmountTypeId equals amt.AmountTypeId
                                 where (mp.CountryId == countryId || mp.CountryId == 0) && mp.StatusId == "AC"
                                 && !mp.AccessType.Any(p => p.AccessTypeId == deviceType)
                                 orderby mp.Rank
                                 select new
                                 {
                                     MacroProducDetails = new Service.ExtendedApi.DataContract.Macro.MacroProduct()
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
                                         RequireQuery = mp.RequireQuery
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
                                                                                      .OrderBy(x => x.MacroProductValueId).Select(x => new Service.ExtendedApi.DataContract.Macro.FieldValues()
                                                                                      {
                                                                                          ValueName = x.ValueName,
                                                                                          Value = x.Value
                                                                                      })
                                                  }),
                                     Channels = (from ch in _db.MacroProductChannel
                                                 where ch.MacroProductId == mp.MacroProductId && ch.CountryId == mp.CountryId
                                                 select new Service.ExtendedApi.DataContract.Macro.Channel()
                                                 {
                                                     MacroProductChannelId = ch.MacroProductChannelId,
                                                     Name = String.Empty,
                                                     Load = ch.Load
                                                 })
                                 }).ToList();

                //Creamos el objeto de respuesta.

                //Los macroproductos estan agrupados por subcategoria.
                foreach (var data in mProducts)
                {
                    var mp = new Service.ExtendedApi.DataContract.Macro.MacroProduct()
                    {
                        MacroProductId = data.MacroProducDetails.MacroProductId,
                        MacroProductName = data.MacroProducDetails.MacroProductName.RemoveAccents(),
                        PercentageFee = data.MacroProducDetails.PercentageFee,
                        FlatFee = data.MacroProducDetails.FlatFee,
                        RePrint = data.MacroProducDetails.RePrint,
                        AmountType = data.MacroProducDetails.AmountType,
                        BalanceType = data.MacroProducDetails.BalanceType,
                        Fields = new List<Service.ExtendedApi.DataContract.Macro.Field>(),
                        Channels = new List<Service.ExtendedApi.DataContract.Macro.Channel>(),
                        Description = data.MacroProducDetails.Description,
                        RequireQuery = _setRequireQuery ? data.MacroProducDetails.RequireQuery : null
                    };

                    var channels = data.Channels.ToList();

                    //Se agrega el nombre del Channel.
                    foreach (var ch in channels)
                    {
                        ch.Name = platformProducts.FirstOrDefault(pp => pp.Key == ch.MacroProductChannelId.ToString()).Value;
                    }

                    //Lista los productos que estan asignados al cliente.
                    var acceptedProducts = platformProducts.Select(p => Convert.ToInt32(p.Key)).ToList();

                    //Elimina los channels que no estan permitidos para el cliente.
                    channels.RemoveAll(ch => !acceptedProducts.Contains(ch.MacroProductChannelId));

                    if (channels.Count > 0)
                    {
                        //var amounts = data.Amounts.ToList().RemoveAccents();
                        var fields = data.FieldList.ToList();

                        //Agrega los Fields del macroproducto.
                        foreach (var x in data.FieldList)
                        {
                            mp.Fields.Add(new Field()
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
                        mp.Channels.AddRange(channels.ToList().RemoveAccents());

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

        internal List<FavoriteAmount> GetFavoriteAmounts(int countryId)
        {
            _db = new MacroProductosEntities();
            var amounts = _db.FavoriteAmounts.Where(f => f.CountryId == countryId).ToList();
            var response = new List<FavoriteAmount>();
            FavoriteAmount favAmount;

            foreach (int mpid in amounts.Select(a => a.MacroProductId).Distinct())
            {
                favAmount = new FavoriteAmount();
                favAmount.MacroProductId = mpid;
                favAmount.Amounts = amounts.Where(a => a.MacroProductId == mpid).OrderBy(a => a.Rank).Select(a => a.Amount).ToList();
                response.Add(favAmount);
            }

            return response;
        }

        internal string GetMacroProductMessage(int countryId, int productId)
        {
            _db = new MacroProductosEntities();

            // Acá toca hacer toda la lógica restante
            var macroProducts = _db.MacroProduct.Where(m => m.CountryId == countryId && m.MacroProductChannel.Any(mpc => mpc.MacroProductChannelId == productId)).ToList();

            if (macroProducts.Count == 1)
                return macroProducts.Single().Message;
            else
                return "";
        }

    }
}