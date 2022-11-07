using System.Web;
using Microsoft.AspNetCore.Mvc;
using QRCoder;
using shunda.Core;

namespace shunda.Admin.Controllers;

public class QrCodeController : Controller
{
    [HttpGet]
    [Route("/admin/qr/code/{code}")]
    public IActionResult Index(string code)
    {
        code = HttpUtility.UrlDecode(code);

        var generator = new QRCodeGenerator();
        var codeData = generator.CreateQrCode(code, QRCodeGenerator.ECCLevel.M, true);

        var qrCode = new PngByteQRCode(codeData);
        var imageBytes = qrCode.GetGraphic(20);

        return new FileContentResult(imageBytes, ContentType.ImagePng);
    }
}