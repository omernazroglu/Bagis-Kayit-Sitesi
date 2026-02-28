using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using UFUKDER_BAGIS.Models;
using UFUKDER_BAGIS.Services.Concrete;
using UFUKDER_BAGIS.Services.Interfaces;

namespace UFUKDER_BAGIS.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IService<Bagislar> _bagislar;
        private readonly IService<RefSutunlar> _refsutunlar;
        private readonly IService<BagisBilgileri> _bagisbilgileri;
        private readonly IService<Referanslar> _referanslar;
        private readonly IUnitOfWork _unitOfWork;
        public HomeController(ILogger<HomeController> logger, IService<Bagislar> bagislar, IService<RefSutunlar> refsutunlar, IService<BagisBilgileri> bagisbilgileri, IService<Referanslar> referanslar, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _bagislar = bagislar;
            _refsutunlar = refsutunlar;
            _bagisbilgileri = bagisbilgileri;
            _referanslar = referanslar;
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Privacy()
        {
            
            var result = await _refsutunlar.GetListAsync();
            if (!result.IsSuccess)
            {
                return Json("sdjgnsdlgs");
            }
            var resultbagislar = await _bagislar.GetListAsync(p => p.Aktif == 1);

            var bagislarids = resultbagislar.List.Select(x => x.Id).ToList();
            var resultbagis = await _bagisbilgileri.GetListAsync(p=>bagislarids.Contains((int)p.BagislarId));

            if (!resultbagis.IsSuccess)
            {
                return Json("dflndfb");

            }
            var model = new BagislarViewModel
            {
                BagisBilgileris = resultbagis.List.ToList(),
                RefSutunlar = result.List.ToList(),
            };
            return View(model);
        }   

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        [HttpPost]
        public async Task<IActionResult> Deneme([FromBody] BagisBilgileri newmodel) {
               using var transaction = await _unitOfWork.BeginTransactionAsync();     
            if (newmodel.SutunlarId == 7)
            {
                var resultRef = await _referanslar.GetItemAsync(p => p.AdSoyad == newmodel.Aciklama);
                if (!resultRef.IsSuccess)
                {
                    return Json(new { success = false });

                }
                var resultBagis = await _bagislar.GetItemAsync(p => p.Id == newmodel.BagislarId);
                if (!resultBagis.IsSuccess)
                {
                    return Json(new { success = false });

                }
                resultBagis.Item.ReferansId = resultRef.Item.Id;
                var resultInsertBagis = await _bagislar.UpdateTransactionalAsync(resultBagis.Item);
            }

            var result = await _bagisbilgileri.GetItemAsync(p => p.SutunlarId == newmodel.SutunlarId && p.BagislarId == newmodel.BagislarId);
            
             if (result.Item == null)
            {
                var model = new BagisBilgileri
                {
                    Aciklama = newmodel.Aciklama,
                    SutunlarId = newmodel.SutunlarId,
                    BagislarId = newmodel.BagislarId,
                };
                var resultInsert = await _bagisbilgileri.InsertTransactionalAsync(model);
                if (!resultInsert.IsSuccess)
                {
                    return Json(new { success = false });
                }

            }
            else
            {

                result.Item.Aciklama = newmodel.Aciklama;
                var resultUpdate = await _bagisbilgileri.UpdateTransactionalAsync(result.Item);
                if (!resultUpdate.IsSuccess)
                {


                    return Json(new { success = false });
                }
            }

            await _unitOfWork.SaveChangesAsync();
            await transaction.CommitAsync();
            var resultBagisBilgisi=await _bagisbilgileri.GetListAsync(p=>p.BagislarId==newmodel.BagislarId);
            foreach(var  item in resultBagisBilgisi.List.OrderBy(p=>p.SutunlarId))
            {
                _logger.LogInformation(newmodel.BagislarId+" "+ item.SutunlarId+" "+item.Aciklama);

            }
            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> EkleDeneme()
        {
            var model = new Bagislar {
                OlusturmaTarihi = DateTime.Now,
                KullaniciId = 1,
                Aktif = 1
            };
         var result=await _bagislar.InsertAsync(model);
            if (!result.IsSuccess)
            {
                return Json(new { success = false });
            }
            for(int i = 1; i < 8; i++)
            {
                _logger.LogInformation(model.Id + " " + i + " " + "-");
            }
            return Json(new { success = true, newId = model.Id });
        }
        [HttpGet]
        public async Task<IActionResult> ReferanslariGetir()
        {
            var result=await _referanslar.GetListAsync();
            if (!result.IsSuccess)
            {  return Json(new { success = false }); }

            return Json(result.List);

        }
            
        [HttpPost]
        public async Task<IActionResult> ReferansEkle([FromBody] Referanslar referans)
        {
            var result = await _referanslar.GetItemAsync(p => p.AdSoyad == referans.AdSoyad);
            if (result.Item != null)
            {
                return Json(new { success = false , message="Bu ad ve soyad ile kayýtlý bir referans mevcut!"});
            }
            var resultInsert= await _referanslar.InsertAsync(referans);
            if (!resultInsert.IsSuccess)
            {
                return Json(new { success = false, message = "Kayýt sýrasýnda bir hata oluþtu!" });
            }
            return Json(new { success = true });

        }

        [HttpPost]
        public async Task<IActionResult> Sil([FromBody] Bagislar bagis)
        {
            var result = await _bagislar.GetItemAsync(p => p.Id == bagis.Id);
            if (!result.IsSuccess)
            {
                return Json(new { success = false });

            }
            result.Item.Aktif = 0;
            var resultUpdate = await _bagislar.UpdateAsync(result.Item);
            if (!resultUpdate.IsSuccess)
            {
                return Json(new { success = false });
            }

            return Json(new { success = true });
        }

        [HttpGet]
        public async Task<IActionResult> GetLogs(int id)
        {
            var resultbagis = new List<BagisBilgileri>();
            var tarihler = new List<string>();

            var result = await _refsutunlar.GetListAsync();
            if (!result.IsSuccess)
            {
                return Json("Hata");
            }

            var path = Path.Combine(Directory.GetCurrentDirectory(), "Logs");

            if (!Directory.Exists(path))
                return Content("Log klasörü bulunamadý");

            var files = Directory.GetFiles(path, "*.txt")
                                 .OrderBy(f => f); // en yeni dosyadan baþla

            foreach (var file in files)
            {
                using (var stream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (var reader = new StreamReader(stream))
                {
                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();

                        if (string.IsNullOrWhiteSpace(line))
                            continue;

                        var parts = line.Split(' ', 6, StringSplitOptions.RemoveEmptyEntries);

                        if (parts.Length < 6)
                            continue;

                        string date = parts[0];
                        string time = parts[1].Substring(0, 5);
                        string splitId = parts[4];

                        if (!int.TryParse(splitId, out int parsedId))
                            continue;

                        if (parsedId != id)
                            continue;

                        string rest = parts[5];
                        int firstSpace = rest.IndexOf(' ');
                        string sutun = firstSpace > 0 ? rest.Substring(0, firstSpace) : rest;
                        string name = firstSpace > 0 ? rest.Substring(firstSpace + 1) : "";

                        if (!int.TryParse(sutun, out int parsedSutun))
                            continue;

                        var splitModel = new BagisBilgileri
                        {
                            BagislarId = parsedId,
                            SutunlarId = parsedSutun,
                            Aciklama = name
                        };

                        if (parsedSutun == 1)
                        {
                            tarihler.Add($"{date} {time}");
                        }

                        resultbagis.Add(splitModel);
                    }
                }
            }

            var model = new LoglarViewModel
            {
                BagisBilgileris = resultbagis,
                RefSutunlar = result.List.ToList(),
                Tarihs = tarihler
            };

            return PartialView("~/Views/Home/Deneme.cshtml", model);
        }
            [HttpPost]
        public async Task<IActionResult> GeriDon([FromBody] List<BagisBilgisiDto> gelenModeller)
        {
            if (gelenModeller == null || !gelenModeller.Any())
            {
                return BadRequest("Veri boþ geldi.");
            }
            using var transaction = await _unitOfWork.BeginTransactionAsync();

            foreach (var item in gelenModeller)
            {
                if (item.SutunlarId == 7)
                {
                    var resultRef = await _referanslar.GetItemAsync(p => p.AdSoyad == item.Aciklama);
                    if (!resultRef.IsSuccess)
                    {
                        return Json(new { success = false });

                    }
                    var resultBagis = await _bagislar.GetItemAsync(p => p.Id == item.BagislarId);
                    if (!resultBagis.IsSuccess)
                    {
                        return Json(new { success = false });

                    }
                    if (resultBagis.Item.ReferansId != resultRef.Item.Id)
                    {
                        resultBagis.Item.ReferansId = resultRef.Item.Id;
                        var resultUpdateBagis = await _bagislar.UpdateTransactionalAsync(resultBagis.Item);
                    }
                }


              
                var result=await _bagisbilgileri.GetItemAsync(p=>p.BagislarId == item.BagislarId && p.SutunlarId==item.SutunlarId);
                if (result.Item == null)
                {
                    var model = new BagisBilgileri
                    {
                        Aciklama = item.Aciklama,
                        SutunlarId = item.SutunlarId,
                        BagislarId = item.BagislarId,
                    };
                    var resultInsert = await _bagisbilgileri.InsertTransactionalAsync(model);
                    if (!resultInsert.IsSuccess)
                    {
                        return Json(new { success = false });
                    }

                }
                else
                {

                    result.Item.Aciklama = item.Aciklama;
                    var resultUpdate = await _bagisbilgileri.UpdateTransactionalAsync(result.Item);
                    if (!resultUpdate.IsSuccess)
                    {


                        return Json(new { success = false });
                    }
                }
            }
            await _unitOfWork.SaveChangesAsync();
            await transaction.CommitAsync();
            return Json(new { success = true, message = "Veriler alýndý" });
        }

        // Yardýmcý DTO sýnýfý (Modelinize göre uyarlayýn)
        public class BagisBilgisiDto
        {
            public int SutunlarId { get; set; }
            public string Aciklama { get; set; }
            public int BagislarId { get; set; }
        }


    }
}
