using System.Text;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using Net.Codecrete.QrCodeGenerator;
using shunda.Core;

namespace shunda.Admin.Controllers;

public class QrCodeController : Controller
{
    [HttpGet]
    [Route("/admin/qr/code/{code}")]
    public IActionResult Index(string code)
    {
        code = HttpUtility.UrlDecode(code);

        // using QRCoder;  https://github.com/codebude/QRCoder/
        // var generator = new QRCodeGenerator();
        // var codeData = generator.CreateQrCode(code, QRCodeGenerator.ECCLevel.M, true);
        //
        // var qrCode = new PngByteQRCode(codeData);
        // var imageBytes = qrCode.GetGraphic(20);
        //
        // return new FileContentResult(imageBytes, ContentType.ImagePng);

        var qrCode = QrCode.EncodeText(code, QrCode.Ecc.Medium);
        string svg = qrCode.ToSvgString(4, "black", "white");
        return new FileContentResult(Encoding.UTF8.GetBytes(svg), ContentType.Svg);
    }
}