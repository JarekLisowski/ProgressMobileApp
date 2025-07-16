//using Progress.Domain.Navireo;

//namespace Progress.Navireo.Managers
//{
//  /// <summary>
//  /// Odpowiada za konfigurację dostaw (mapowanie)
//  /// </summary>
//  public class DeliveryManager
//  {
//    Database.NavireoDbContext dbContext;
//    /// <summary>
//    /// Zwraca listę sposobów dostawy
//    /// </summary>
//    /// <returns></returns>
//    public IEnumerable<DeliveryType> GetList()
//    {
//      {
//        var result = new List<DeliveryType>();
//        var deliveryList = from delivery in dbContext.IfxApiSposobDostawies
//                           join twTowar in dbContext.TwTowars on delivery.TwId equals twTowar.TwId
//                           join twCena in dbContext.TwCenas on delivery.TwId equals twCena.TcIdTowar
//                           select new { delivery, twCena, twTowar };

//        result = deliveryList.Select(it => new DeliveryType
//        {
//          Id = it.delivery.TwId,
//          IsUpdated = it.delivery.Aktywny,
//          IsDeleted = !it.delivery.Aktywny,
//          Name = it.delivery.Nazwa,
//          PriceNet = it.twCena.TcCenaNetto2 ?? 0,
//          PriceGross = it.twCena.TcCenaBrutto2 ?? 0,
//          Tax = it.twTowar.TwIdVatSpNavigation != null ? new Tax
//          {
//            Id = it.twTowar.TwIdVatSpNavigation.VatId,
//            Rate = it.twTowar.TwIdVatSpNavigation.VatStawka
//          } : null
//        }).ToList();
//        return result;
//      }
//    }


//    /// <summary>
//    /// Zwraca listę sposobów dostawy
//    /// </summary>
//    /// <returns></returns>
//    //public IEnumerable<DeliveryNavireo> GetMappedList()
//    //{
//    //    using (var dbConnection = new Model.NavireoEntities())
//    //    {
//    //        var result = new List<DeliveryNavireo>();

//    //        var delivery = from ifx in dbConnection.IFx_ApiSposobDostawy
//    //                       join tw in dbConnection.tw__Towar on ifx.Id equals tw.tw_Id
//    //                       select new { ifx, code = tw.tw_Symbol };

//    //        result = delivery.Select(it => new DeliveryNavireo
//    //        {
//    //            Id = it.ifx.Id,
//    //            IsUpdated = it.ifx.Aktywny,
//    //            IsDeleted = !it.ifx.Aktywny,
//    //            Name = it.ifx.Nazwa,
//    //            NavireoId = it.ifx.tw_Id,
//    //            Code = it.code
//    //        }).ToList();
//    //        return result;
//    //    }
//    //}

//    /// <summary>
//    /// Zwraca listę dostępnych usług z Navireo
//    /// </summary>
//    /// <returns>Lista dostępnych płatności</returns>
//    //public IEnumerable<DeliveryType> GetAvailable()
//    //{
//    //    using (var dbConnection = new Model.NavireoEntities())
//    //    {
//    //        var result = new List<DeliveryType>();

//    //        var config = dbConnection.IFx_ApiUstawienia.First();
//    //        var availableServices = dbConnection.tw__Towar.Where(x => x.tw_Rodzaj != 1);
//    //        foreach (var item in availableServices)
//    //        {
//    //            result.Add(new DeliveryType
//    //            {
//    //                Id = item.tw_Id,
//    //                Name = item.tw_Nazwa,
//    //                PriceNet = item.tw_Cena.First().tc_CenaNetto2 ?? 0,
//    //                PriceGross = item.tw_Cena.First().tc_CenaNetto2 ?? 0,
//    //                Tax = new Tax
//    //                {
//    //                    Id = item.tw_IdVatSp ?? 0,
//    //                    Rate = item.sl_StawkaVAT.vat_Stawka
//    //                }
//    //            });
//    //        }
//    //        return result;
//    //    }
//    //}

//    /// <summary>
//    /// Aktualizacja mapowania dostawy
//    /// </summary>
//    /// <param name="delivery">Dostawa</param>
//    //public void Update(DeliveryNavireo delivery)
//    //{
//    //    using (var dbConnection = new Model.NavireoEntities())
//    //    {
//    //        var ifx = dbConnection.IFx_ApiSposobDostawy.FirstOrDefault(x => x.Id == delivery.Id);
//    //        if (delivery.IsNew)
//    //        {
//    //            var exist = dbConnection.IFx_ApiSposobDostawy.FirstOrDefault(x => x.Aktywny && x.tw_Id == delivery.NavireoId);
//    //            if (exist != null) throw new Exception("Wybrany sposób dostawy jest już przypisany");
//    //            ifx = new Model.IFx_ApiSposobDostawy
//    //            {
//    //                tw_Id = delivery.NavireoId,
//    //                Nazwa = delivery.Name,
//    //                Aktywny = true
//    //            };
//    //            dbConnection.IFx_ApiSposobDostawy.Add(ifx);
//    //        }
//    //        if (delivery.IsUpdated)
//    //        {
//    //            if (ifx == null) throw new Exception("Brak dostawy o Id: {0} w tabeli IFx_ApiSposobDostawy");
//    //            ifx.Nazwa = delivery.Name;
//    //            ifx.Aktywny = true;
//    //            if (ifx.tw_Id != delivery.NavireoId)
//    //            {
//    //                var exist = dbConnection.IFx_ApiFormaPlatnosci.FirstOrDefault(x => x.Aktywna && x.fp_Id == delivery.NavireoId);
//    //                if (exist != null) throw new Exception("Wybrany sposób dostawy jest już przypisany");
//    //            }
//    //            ifx.tw_Id = delivery.NavireoId;
//    //        }
//    //        if (delivery.IsDeleted)
//    //        {
//    //            if (ifx == null) throw new Exception("Brak platności o Id: {0} w tabeli IFx_ApiFormaPlatnosci");
//    //            ifx.Aktywny = false;
//    //        }
//    //        dbConnection.SaveChanges();
//    //        if (!delivery.IsDeleted) delivery.Id = ifx.Id;
//    //    }
//    //}
//  }
//}
