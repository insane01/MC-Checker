using System.Web.Script.Serialization;

namespace MC_Checker
{
    class Json
    {
        public static dynamic Deserialize(string json)
        {
            var jss = new JavaScriptSerializer();
            var sData = jss.Deserialize<dynamic>(json);

            return sData;
        }

        public static string Serialize(dynamic json)
        {
            var jss = new JavaScriptSerializer();
            var sData = jss.Serialize(json);

            return sData;
        }
    }
}
