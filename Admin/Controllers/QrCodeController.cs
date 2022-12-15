using System.Text;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using Net.Codecrete.QrCodeGenerator;
using shunda.Core;
using shunda.Core.QrCode;


namespace shunda.Admin.Controllers;

public class QrCodeController : Controller
{
    [HttpGet]
    [Route("/admin/qr/code/svg/{code}")]
    public IActionResult Svg(string code)
    {
        code = HttpUtility.UrlDecode(code);

        // https://github.com/manuelbl/QrCodeGenerator
        var qrCode = QrCode.EncodeText(code, QrCode.Ecc.Medium);
        string svg = qrCode.ToSvgString(4, "#000000", "#ffffff");
        return new FileContentResult(Encoding.UTF8.GetBytes(svg), ContentType.Svg);
    }

    [HttpGet]
    [Route("/admin/qr/code/png/{code}")]
    public IActionResult Png(string code)
    {
        code = HttpUtility.UrlDecode(code);

        // https://github.com/manuelbl/QrCodeGenerator
        var qrCode = QrCode.EncodeText(code, QrCode.Ecc.Medium);
        byte[] png = qrCode.ToPng(20, 0);
        return new FileContentResult(png, ContentType.ImagePng);
    }
}