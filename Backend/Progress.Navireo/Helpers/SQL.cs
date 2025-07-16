namespace Progress.Navireo.Helpers
{
    public static class SQL
    {
        public static int GetCountryId(Database.NavireoDbContext dbConnection, string countryCode)
        {
            var country = dbConnection.SlPanstwos.FirstOrDefault(x => x.PaKodPanstwaUe == countryCode);
            if (country == null) throw new Exception(string.Format("Brak Panstwa o kodzie: {0} w bazie Navireo", countryCode));
            return country.PaId;
        }

        //public static Country GetCountryObject(Database.NavireoDbContext dbContext, int id)
        //{
        //    {
        //        var country = dbContext.sl_Panstwo.FirstOrDefault(x => x.pa_Id == id);
        //        if (country == null) throw new Exception(string.Format("Panstwo o Id: {0} nie istnieje w bazie Navireo", id));
        //        return new Country
        //        {
        //            Id = country.pa_Id,
        //            Name = country.pa_Nazwa,
        //            Code = country.pa_KodPanstwaUE
        //        };
        //    }
        //}

        //public static List<Category> WczytajCechyTowaru(int twId, SqlConnection sqlConnection)
        //{
        //    List<Category> cechy = new List<Category>();
        //    string query = "SELECT ctw_Nazwa from sl_CechaTw, tw_CechaTw Where sl_CechaTw.ctw_Id = tw_CechaTw.cht_IdCecha "
        //                    + "and tw_CechaTw.cht_IdTowar = {0}";
        //    query = string.Format(query, twId);
        //    SqlCommand command = new SqlCommand(query, sqlConnection);
        //    try
        //    {
        //        SqlDataReader reader = command.ExecuteReader();
        //        while (reader.Read())
        //        {
        //            cechy.Add(new Category
        //            {
        //                Description = new Description
        //                {
        //                    Name = Convert.ToString(reader[0])
        //                }
        //            });
        //        }
        //    }
        //    catch (Exception)
        //    { }
        //    return cechy;
        //}

        //public static Tax PobierzVat(int id, Model.NavireoEntities dbContext)
        //{
        //    var tax = dbContext.sl_StawkaVAT.First(x => x.vat_Id == id);
        //    return new Tax
        //    {
        //        Id = tax.vat_Id,
        //        Rate = tax.vat_Stawka,
        //        Name = tax.vat_Nazwa
        //    };
        //}



        //public static void UstawJakoJednorazowy(int khId, SqlConnection sqlConnection)
        //{
        //    string commandString = string.Format("UPDATE kh__Kontrahent SET kh_Symbol = '********************', kh_Rodzaj = 0, kh_Cena = NULL, kh_Jednorazowy = 1, kh_CRM = 0 WHERE kh_Id = {0} ", khId);
        //    using (SqlCommand sqlCommand = new SqlCommand(commandString, sqlConnection))
        //    {
        //        int res = sqlCommand.ExecuteNonQuery();
        //        commandString = string.Format("UPDATE adr__Ewid SET adr_Symbol = '********************' WHERE adr_IdObiektu = {0} and adr_TypAdresu = 1 ", khId);
        //        sqlCommand.CommandText = commandString;
        //        res = sqlCommand.ExecuteNonQuery();
        //        if (res > 0)
        //        {
        //            commandString = string.Format("Select adr_Id FROM adr__Ewid WHERE adr_IdObiektu = {0} and adr_TypAdresu = 1 ", khId);
        //            sqlCommand.CommandText = commandString;
        //            var adrId = sqlCommand.ExecuteScalar();
        //            commandString = string.Format("UPDATE adr_Historia SET adrh_Symbol = '********************' WHERE adrh_IdAdresu = {0} ", adrId);
        //            sqlCommand.CommandText = commandString;
        //            sqlCommand.ExecuteNonQuery();
        //        }
        //    }
        //}

        //public static decimal PobierzStanTowaru(int towarId, int magazynId, Model.NavireoEntities dbConnection)
        //{
        //    var tw_stan = dbConnection.tw_Stan.FirstOrDefault(x => x.st_TowId == towarId && x.st_MagId == magazynId);
        //    if (tw_stan == null) throw new Exception(string.Format("Error in tw_Stan for st_TowId = {0} and st_MagId = {1}", towarId, magazynId));

        //    return tw_stan.st_Stan;
        //}
                
    }
}
