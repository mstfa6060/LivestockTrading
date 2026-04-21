using System.util;
using Common.Contracts.Queue.Models;
using Common.Services.Environment;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.codec;
using iTextSharp.text.pdf.qrcode;

namespace Common.Services.FileOperations;

public class PdfService
{
	private readonly EnvironmentService _environmentService;
	public PdfService(EnvironmentService environmentService)
	{
		_environmentService = environmentService;
	}
	public byte[] AddedQrCode(byte[] fileContent, List<ReplacementItem> replacementData)
	{
		var creatorOrganizationName = replacementData.FirstOrDefault(r => r.Key == "X-Creator-Organization").Values.FirstOrDefault();
		var creatorUserName = replacementData.FirstOrDefault(r => r.Key == "AUTHOR_NAMESURNAME").Values.FirstOrDefault();
		var documentNumber = replacementData.FirstOrDefault(r => r.Key == "DOCUMENT_NO").Values.FirstOrDefault();
		var accessCode = replacementData.Where(s => s.Key == "ACCESS_CODE").Select(s => s.Values.FirstOrDefault()).FirstOrDefault();

		var url = _environmentService.Environment == CustomEnvironments.Production ? "https://belgedogrulama.toprak.tkholding.com.tr/?doc_id=" : "https://belgedogrulama-test.toprak.tkholding.com.tr/?doc_id=";
		var qrParameter = "isActive={1} size={100} alignH={right} alignV={bottom} alignMarginH={100} alignMarginV={1} verifyUrl={" + url + "}" + accessCode;
		NVParserOld QrCodeOptions = new NVParserOld(qrParameter);
		var isActive = QrCodeOptions.GetValueOld("isActive") == "1";

		var size = 173; //Barkod görselinin (beyaz)kenar boşluğu oranı her boyutta farklılık gösterdiği için 170-200 aralığında optimum sonucu 173 değeri verdi.   
		var isAlignLeft = QrCodeOptions.GetValueOld("alignH").ToLower() == "left" ? true : false;
		var isAlignTop = QrCodeOptions.GetValueOld("alignV").ToLower() == "top" ? true : false;
		var alignMarginH = Convert.ToInt32(QrCodeOptions.GetValueOld("alignMarginH"));
		var alignMarginV = Convert.ToInt32(QrCodeOptions.GetValueOld("alignMarginV"));
		var verifyUrl = QrCodeOptions.GetValueOld("verifyUrl") + accessCode;
		string preText = @"Belgeyi Üreten İdare: {0} " + System.Environment.NewLine + "Belge Sayısı: {1} " + System.Environment.NewLine + "Belge Doğrulama Adresi: {2} " + System.Environment.NewLine + "Belge Doğrulama Kodu: {3}" + System.Environment.NewLine;

		var qrText = string.Format(preText,
									creatorOrganizationName + " " + creatorUserName,
									documentNumber,
									verifyUrl,
									accessCode
									);

		// qrText = "Belgeyi Üreten İdare: TK TEKNOLOJİ A.Ş. TEST ÜRÜN YÖNETİM DİREKTÖRLÜĞÜ Belge Sayısı: E.Organization52 - 849.01 - 1385 / 2319 Belge Doğrulama Adresi: http://belgedogrulama.tkteknoloji.com.tr/?accesscode=587f01669f744da99f5a33aaf43d23bb Belge Doğrulama Kodu: 587f01669f744da99f5a33aaf43d23bb";
		var hints = new NullValueDictionary<iTextSharp.text.pdf.qrcode.EncodeHintType, object>() { { EncodeHintType.CHARACTER_SET, "UTF-8" } };

		var qrCodeImage = CreateQrCodeImage(qrText, size, size, hints);

		using (PdfReader pdfReader = new PdfReader(fileContent))
		{
			MemoryStream ms = new MemoryStream();
			PdfStamper pdfStamper = new PdfStamper(pdfReader, ms);

			for (int i = 1; i <= pdfReader.NumberOfPages; i++)
			{
				var pageSize = pdfReader.GetPageSizeWithRotation(i);

				var baseXCoordinate = isAlignLeft ? 0 + alignMarginH : pageSize.Width - alignMarginH;
				var baseYCoordinate = isAlignTop ? pageSize.Height - alignMarginV : 0 + alignMarginV;

				qrCodeImage.SetAbsolutePosition(baseXCoordinate, baseYCoordinate);

				PdfContentByte overContent = pdfStamper.GetOverContent(i);
				var scaledSize = Convert.ToInt32(QrCodeOptions.GetValueOld("size"));
				// var scaledSize = 100;
				qrCodeImage.ScaleToFit(scaledSize, scaledSize);
				overContent.AddImage(qrCodeImage);
			}

			pdfStamper.Close();
			pdfReader.Close();
			byte[] result = ms.ToArray();

			return result;
		}
	}


	private static Image CreateQrCodeImage(string content, int _width, int _height, System.util.INullValueDictionary<iTextSharp.text.pdf.qrcode.EncodeHintType, object> hints = null)
	{
		var qrCodeWriter = new QRCodeWriter();
		var byteMatrix = qrCodeWriter.Encode(content, _width, _height, hints);
		var width = byteMatrix.GetWidth();
		var height = byteMatrix.GetHeight();
		var stride = (width + 7) / 8;
		var bitMatrix = new byte[stride * height];
		var byteMatrixArray = byteMatrix.GetArray();

		for (var y = 0; y < height; ++y)
		{
			var line = byteMatrixArray[y];
			for (var x = 0; x < width; ++x)
			{
				if (line[x] != 0)
				{
					var offset = stride * y + x / 8;
					bitMatrix[offset] |= (byte)(0x80 >> (x % 8));
				}
			}
		}

		var encodedImage = Ccittg4Encoder.Compress(bitMatrix, byteMatrix.GetWidth(), byteMatrix.GetHeight());
		var qrcodeImage = Image.GetInstance(byteMatrix.GetWidth(), byteMatrix.GetHeight(), false, Element.CCITTG4,
											Element.CCITT_BLACKIS1, encodedImage, null);

		return qrcodeImage;
	}




}