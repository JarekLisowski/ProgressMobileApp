namespace Progress.Navireo.Helpers
{
    public static class Parameters
    {
        public static int GetParameterInt(string name, Database.NavireoDbContext dbConnection)
        {
            var parameter = dbConnection.IfxParameters.FirstOrDefault(x => x.ParName == name);
            if (parameter == null && parameter.ParValueInt != null) throw new Exception(string.Format("Brak parametru int w bazie o nazwie: {0}", name));
            return (int)parameter.ParValueInt;
        }

        public static string GetParameterString(string name, Database.NavireoDbContext dbConnection)
        {
            var parameter = dbConnection.IfxParameters.FirstOrDefault(x => x.ParName == name);
            if (parameter == null && parameter.ParValueString != null) throw new Exception(string.Format("Brak parametru string w bazie o nazwie: {0}", name));
            return parameter.ParValueString;
        }

        public static decimal GetParameterDecimal(string name, Database.NavireoDbContext dbConnection)
        {
            var parameter = dbConnection.IfxParameters.FirstOrDefault(x => x.ParName == name);
            if (parameter == null && parameter.ParValueDecimal != null) throw new Exception(string.Format("Brak parametru decimal w bazie o nazwie: {0}", name));
            return (decimal)parameter.ParValueDecimal;
        }

        public static void SaveParameter(string name, int value, Database.NavireoDbContext dbConnection)
        {
            var parameter = dbConnection.IfxParameters.FirstOrDefault(x => x.ParName == name);
            if (parameter == null) throw new Exception(string.Format("brak parametru : '{0}'", name));
            parameter.ParValueInt = value;
            dbConnection.SaveChanges();
        }

        public static void SaveParameter(string name, decimal value, Database.NavireoDbContext dbConnection)
        {
            var parameter = dbConnection.IfxParameters.FirstOrDefault(x => x.ParName == name);
            if (parameter == null) throw new Exception(string.Format("brak parametru : '{0}'", name));
            parameter.ParValueDecimal = value;
            dbConnection.SaveChanges();
        }

        public static void SaveParameter(string name, string value, Database.NavireoDbContext dbConnection)
        {
            var parameter = dbConnection.IfxParameters.FirstOrDefault(x => x.ParName == name);
            if (parameter == null) throw new Exception(string.Format("brak parametru : '{0}'", name));
            parameter.ParValueString = value;
            dbConnection.SaveChanges();
        }
    }
}
