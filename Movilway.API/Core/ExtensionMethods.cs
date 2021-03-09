using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using Movilway.API.Service.ExtendedApi.DataContract.Macro;
using System.Data.SqlClient;
using Movilway.API.Core;

namespace Movilway.API.Core
{
  
    public static class ExtensionMethods
    {
    
        public static string RemoveAccents(this string inputString)
        {
            var sb = new StringBuilder();

            var normalizedString = inputString.Normalize(NormalizationForm.FormD);

            foreach (var t in (from t in normalizedString
                               let uc = CharUnicodeInfo.GetUnicodeCategory(t)
                               where uc != UnicodeCategory.NonSpacingMark
                               select t))
            {
                sb.Append(t);
            }

            return sb.ToString().Normalize(NormalizationForm.FormC);
        }

        public static List<Field> RemoveAccents(this List<Field> fields)
        {
            var newFields = new List<Field>();

            if (fields.Any())
            {
                newFields = fields.Select(f => new Field()
                                                   {
                                                       Id = f.Id,
                                                       Name = f.Name.RemoveAccents(),
                                                       Type = f.Type.RemoveAccents(),
                                                       Length = f.Length,
                                                       Format = f.Format,
                                                       Description = f.Description.RemoveAccents(),
                                                       Mandatory = f.Mandatory,
                                                       MapValueNameTo = f.MapValueNameTo,
                                                       MapValueTo = f.MapValueTo,
                                                       Min = f.Min,
                                                       Max = f.Max
                                                   }).ToList();
            }

            return newFields;
        }

        public static List<Field> AddAmount(this List<Field> fields, List<AmountEntity> amounts, String AmountMapValueName, String AmountMapValue)
        {
            if (amounts.Any())
            {
                var amId = fields.Any() ? fields.Max(f => f.Id) + 1 : 0;

                var amount = new Field()
                                 {
                                     Id = amId,
                                     Name = "Amount",
                                     Type = "Integer",
                                     Description = "Montos Admitidos",
                                     MapValueTo = AmountMapValue,
                                     MapValueNameTo = AmountMapValueName,
                                     Format = String.Empty,
                                     Length = 0
                                 };

                foreach (var amountEntity in amounts)
                {
                    switch (amountEntity.AmountName)
                    {
                        case "Min":
                            amount.Min = amountEntity.Amount;
                            break;

                        case "Max":
                            amount.Max = amountEntity.Amount;
                            break;

                        default:
                            //amount.Values.Add(new FieldValues() { Key = amountEntity.AmountName, Value = amountEntity.Amount.ToString() });
                            break;

                    }
                }

                fields.Add(amount);
            }

            return fields;
        }

        public static List<AmountEntity> RemoveAccents(this List<AmountEntity> amounts)
        {
            var newAmounts = new List<AmountEntity>();

            if (amounts.Any())
            {
                newAmounts = amounts.Select(a => new AmountEntity()
                {
                    Amount = a.Amount,
                    AmountName = a.AmountName.RemoveAccents()
                }).ToList();
            }

            return newAmounts;
        }

        public static List<Channel> RemoveAccents(this List<Channel> channels)
        {
            var newChannels = new List<Channel>();

            if (channels.Any())
            {
                newChannels = channels.Select(c => new Channel()
                                                       {
                                                           Name = c.Name.RemoveAccents(),
                                                           MacroProductChannelId = c.MacroProductChannelId
                                                       }).ToList();
            }

            return newChannels;
        }
    }


    public enum Countries {ARGENTINA,CHILE,COLOMBIA,ECUADOR,ESPANIA,GUATEMALA,MEXICO,NICARAGUA,PANAMA,PERU,RDOMINICANA,USA,VENEZUELA} 
    public static class GeneralSettings
    {
        public static Countries GetCurrentContry()
        {
         ;
            //apesar que argentina esta cerrado se coloca como primera opcion si para inicializar la variable
            Countries result = Countries.ARGENTINA;
            string[] prefix = { "ar","cl","co","ec","es","gt","mx","ni","pa", "pe","do","us","ve"};
            foreach (System.Configuration.SettingsProperty currentProperty in Properties.Settings.Default.Properties)
            {
              string val =  Properties.Settings.Default[currentProperty.Name].ToString();
              bool bandera = false;
              int i = 0;
              for (; i < prefix.Length && !bandera;)
              {
                     bandera =  val.IndexOf("-"+prefix[i]+"-", StringComparison.InvariantCultureIgnoreCase)> 0;
                     if (!bandera)
                         i++;
                  }




              if (bandera) {
                  result =GetById(i);
                  break;
              }
            }
            return result;
        }

         private static Countries GetById( int index)
        {
            Countries result = Countries.ARGENTINA;
                int i = 0;
            foreach (Countries child in Enum.GetValues(typeof(Countries)))
            {
                if (i == index)
                {
                    result = child;
                    break;
                }
                else
                    i++;
            }


            return result;

        }

       
    }
    
}