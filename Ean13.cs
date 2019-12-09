using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;

public class Ean13
{
	private string _sName = "EAN13";

	private float _fMinimumAllowableScale = 0.2f;

	private float _fMaximumAllowableScale = 2f;

	private float _fWidth = 42.29f;

	private float _fHeight = 14.23f;

	private float _fFontSize = 10f;

	private float _fScale = 1f;

	private float _marginLeft = 4f;

	private float _marginTop = 0.7f;

	private float _marginBarCode = -0.4f;

	private string[] _aOddLeft = new string[10]
	{
		"0001101",
		"0011001",
		"0010011",
		"0111101",
		"0100011",
		"0110001",
		"0101111",
		"0111011",
		"0110111",
		"0001011"
	};

	private string[] _aEvenLeft = new string[10]
	{
		"0100111",
		"0110011",
		"0011011",
		"0100001",
		"0011101",
		"0111001",
		"0000101",
		"0010001",
		"0001001",
		"0010111"
	};

	private string[] _aRight = new string[10]
	{
		"1110010",
		"1100110",
		"1101100",
		"1000010",
		"1011100",
		"1001110",
		"1010000",
		"1000100",
		"1001000",
		"1110100"
	};

	private string _sQuiteZone = "000000000";

	private string _sLeadTail = "101";

	private string _sSeparator = "01010";

	private string _sCountryCode = "00";

	private string _sManufacturerCode;

	private string _sProductCode;

	private string _sChecksumDigit;

	public string Name => _sName;

	public float MinimumAllowableScale => _fMinimumAllowableScale;

	public float MaximumAllowableScale => _fMaximumAllowableScale;

	public float Width
	{
		get
		{
			return _fWidth;
		}
		set
		{
			_fWidth = value;
		}
	}

	public float Height
	{
		get
		{
			return _fHeight;
		}
		set
		{
			_fHeight = value;
		}
	}

	public float FontSize
	{
		get
		{
			return _fFontSize;
		}
		set
		{
			_fFontSize = value;
		}
	}

	public float Scale
	{
		get
		{
			return _fScale;
		}
		set
		{
			_fScale = value;
		}
	}

	public float MarginLeft
	{
		get
		{
			return _marginLeft;
		}
		set
		{
			_marginLeft = value;
		}
	}

	public float MarginTop
	{
		get
		{
			return _marginTop;
		}
		set
		{
			_marginTop = value;
		}
	}

	public float MarginBarCode
	{
		get
		{
			return _marginBarCode;
		}
		set
		{
			_marginBarCode = value;
		}
	}

	public string CountryCode
	{
		get
		{
			return _sCountryCode;
		}
		set
		{
			while (value.Length < 2)
			{
				value = "0" + value;
			}
			_sCountryCode = value;
		}
	}

	public string ManufacturerCode
	{
		get
		{
			return _sManufacturerCode;
		}
		set
		{
			_sManufacturerCode = value;
		}
	}

	public string ProductCode
	{
		get
		{
			return _sProductCode;
		}
		set
		{
			_sProductCode = value;
		}
	}

	public string ChecksumDigit
	{
		get
		{
			return _sChecksumDigit;
		}
		set
		{
			int num = Convert.ToInt32(value);
			if (num < 0 || num > 9)
			{
				throw new Exception("The Check Digit must be between 0 and 9.");
			}
			_sChecksumDigit = value;
		}
	}

	public Ean13()
	{
	}

	public Ean13(string mfgNumber, string productId)
	{
		CountryCode = "00";
		ManufacturerCode = mfgNumber;
		ProductCode = productId;
		CalculateChecksumDigit();
	}

	public Ean13(string countryCode, string mfgNumber, string productId)
	{
		CountryCode = countryCode;
		ManufacturerCode = mfgNumber;
		ProductCode = productId;
		CalculateChecksumDigit();
	}

	public Ean13(string countryCode, string mfgNumber, string productId, string checkDigit)
	{
		CountryCode = countryCode;
		ManufacturerCode = mfgNumber;
		ProductCode = productId;
		ChecksumDigit = checkDigit;
	}

	public Ean13(string countryCode, string mfgNumber, string productId, string checkDigit, float[] EANParams)
	{
		CountryCode = countryCode;
		ManufacturerCode = mfgNumber;
		ProductCode = productId;
		ChecksumDigit = checkDigit;
		_fWidth = EANParams[0];
		_fHeight = EANParams[1];
		_fFontSize = EANParams[2];
		_fScale = EANParams[3];
		_marginLeft = EANParams[4];
		_marginTop = EANParams[5];
		_marginBarCode = EANParams[6];
	}

	public void DrawEan13Barcode(Graphics g, Point pt, float topOffset, float leftOffset, float hScale, float wScale)
	{
		float num = Width * Scale * wScale;
		float num2 = Height * Scale * hScale;
		float num3 = num / 113f;
		GraphicsState gstate = g.Save();
		try
		{
			g.PageUnit = GraphicsUnit.Millimeter;
			g.PageScale = 1f;
			SolidBrush brush = new SolidBrush(Color.Black);
			float num4 = _marginLeft + leftOffset;
			StringBuilder stringBuilder = new StringBuilder();
			StringBuilder stringBuilder2 = new StringBuilder();
			float num5 = pt.X;
			float num6 = (float)pt.Y + _marginTop + topOffset;
			Font font = new Font("Arial", _fFontSize * Scale);
			CalculateChecksumDigit();
			stringBuilder2.AppendFormat("{0}{1}{2}{3}", CountryCode, ManufacturerCode, ProductCode, ChecksumDigit);
			string text = stringBuilder2.ToString();
			string text2 = "";
			text2 = ConvertLeftPattern(text.Substring(0, 7));
			stringBuilder.AppendFormat("{0}{1}{2}{3}{4}{1}{0}", _sQuiteZone, _sLeadTail, text2, _sSeparator, ConvertToDigitPatterns(text.Substring(7), _aRight));
			string text3 = stringBuilder.ToString();
			float height = g.MeasureString(text3, font).Height;
			for (int i = 0; i < stringBuilder.Length; i++)
			{
				if (text3.Substring(i, 1) == "1")
				{
					if (num5 == (float)pt.X)
					{
						num5 = num4;
					}
					if ((i > 12 && i < 55) || (i > 57 && i < 101))
					{
						g.FillRectangle(brush, num4, num6, num3, num2 - height);
					}
					else
					{
						g.FillRectangle(brush, num4, num6, num3, num2 - height / 2f);
					}
				}
				num4 += num3;
			}
			num4 = num5 - g.MeasureString(CountryCode.Substring(0, 1), font).Width;
			float y = num6 + (num2 - height) + _marginBarCode;
			g.DrawString(text.Substring(0, 1), font, brush, new PointF(num4, y));
			num4 += g.MeasureString(text.Substring(0, 1), font).Width + 43f * num3 - g.MeasureString(text.Substring(1, 6), font).Width;
			g.DrawString(text.Substring(1, 6), font, brush, new PointF(num4, y));
			num4 += g.MeasureString(text.Substring(1, 6), font).Width + 11f * num3;
			g.DrawString(text.Substring(7), font, brush, new PointF(num4, y));
		}
		catch
		{
		}
		finally
		{
			g.Restore(gstate);
		}
	}

	public Bitmap CreateBitmap()
	{
		float num = Width * Scale * 100f;
		float num2 = Height * Scale * 100f;
		Bitmap bitmap = new Bitmap((int)num, (int)num2);
		Graphics graphics = Graphics.FromImage(bitmap);
		DrawEan13Barcode(graphics, new Point(0, 0), 0f, 0f, 1f, 1f);
		graphics.Dispose();
		return bitmap;
	}

	private string ConvertLeftPattern(string sLeft)
	{
		switch (sLeft.Substring(0, 1))
		{
		case "0":
			return CountryCode0(sLeft.Substring(1));
		case "1":
			return CountryCode1(sLeft.Substring(1));
		case "2":
			return CountryCode2(sLeft.Substring(1));
		case "3":
			return CountryCode3(sLeft.Substring(1));
		case "4":
			return CountryCode4(sLeft.Substring(1));
		case "5":
			return CountryCode5(sLeft.Substring(1));
		case "6":
			return CountryCode6(sLeft.Substring(1));
		case "7":
			return CountryCode7(sLeft.Substring(1));
		case "8":
			return CountryCode8(sLeft.Substring(1));
		case "9":
			return CountryCode9(sLeft.Substring(1));
		default:
			return "";
		}
	}

	private string CountryCode0(string sLeft)
	{
		return ConvertToDigitPatterns(sLeft, _aOddLeft);
	}

	private string CountryCode1(string sLeft)
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(ConvertToDigitPatterns(sLeft.Substring(0, 1), _aOddLeft));
		stringBuilder.Append(ConvertToDigitPatterns(sLeft.Substring(1, 1), _aOddLeft));
		stringBuilder.Append(ConvertToDigitPatterns(sLeft.Substring(2, 1), _aEvenLeft));
		stringBuilder.Append(ConvertToDigitPatterns(sLeft.Substring(3, 1), _aOddLeft));
		stringBuilder.Append(ConvertToDigitPatterns(sLeft.Substring(4, 1), _aEvenLeft));
		stringBuilder.Append(ConvertToDigitPatterns(sLeft.Substring(5, 1), _aEvenLeft));
		return stringBuilder.ToString();
	}

	private string CountryCode2(string sLeft)
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(ConvertToDigitPatterns(sLeft.Substring(0, 1), _aOddLeft));
		stringBuilder.Append(ConvertToDigitPatterns(sLeft.Substring(1, 1), _aOddLeft));
		stringBuilder.Append(ConvertToDigitPatterns(sLeft.Substring(2, 1), _aEvenLeft));
		stringBuilder.Append(ConvertToDigitPatterns(sLeft.Substring(3, 1), _aEvenLeft));
		stringBuilder.Append(ConvertToDigitPatterns(sLeft.Substring(4, 1), _aOddLeft));
		stringBuilder.Append(ConvertToDigitPatterns(sLeft.Substring(5, 1), _aEvenLeft));
		return stringBuilder.ToString();
	}

	private string CountryCode3(string sLeft)
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(ConvertToDigitPatterns(sLeft.Substring(0, 1), _aOddLeft));
		stringBuilder.Append(ConvertToDigitPatterns(sLeft.Substring(1, 1), _aOddLeft));
		stringBuilder.Append(ConvertToDigitPatterns(sLeft.Substring(2, 1), _aEvenLeft));
		stringBuilder.Append(ConvertToDigitPatterns(sLeft.Substring(3, 1), _aEvenLeft));
		stringBuilder.Append(ConvertToDigitPatterns(sLeft.Substring(4, 1), _aEvenLeft));
		stringBuilder.Append(ConvertToDigitPatterns(sLeft.Substring(5, 1), _aOddLeft));
		return stringBuilder.ToString();
	}

	private string CountryCode4(string sLeft)
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(ConvertToDigitPatterns(sLeft.Substring(0, 1), _aOddLeft));
		stringBuilder.Append(ConvertToDigitPatterns(sLeft.Substring(1, 1), _aEvenLeft));
		stringBuilder.Append(ConvertToDigitPatterns(sLeft.Substring(2, 1), _aOddLeft));
		stringBuilder.Append(ConvertToDigitPatterns(sLeft.Substring(3, 1), _aOddLeft));
		stringBuilder.Append(ConvertToDigitPatterns(sLeft.Substring(4, 1), _aEvenLeft));
		stringBuilder.Append(ConvertToDigitPatterns(sLeft.Substring(5, 1), _aEvenLeft));
		return stringBuilder.ToString();
	}

	private string CountryCode5(string sLeft)
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(ConvertToDigitPatterns(sLeft.Substring(0, 1), _aOddLeft));
		stringBuilder.Append(ConvertToDigitPatterns(sLeft.Substring(1, 1), _aEvenLeft));
		stringBuilder.Append(ConvertToDigitPatterns(sLeft.Substring(2, 1), _aEvenLeft));
		stringBuilder.Append(ConvertToDigitPatterns(sLeft.Substring(3, 1), _aOddLeft));
		stringBuilder.Append(ConvertToDigitPatterns(sLeft.Substring(4, 1), _aOddLeft));
		stringBuilder.Append(ConvertToDigitPatterns(sLeft.Substring(5, 1), _aEvenLeft));
		return stringBuilder.ToString();
	}

	private string CountryCode6(string sLeft)
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(ConvertToDigitPatterns(sLeft.Substring(0, 1), _aOddLeft));
		stringBuilder.Append(ConvertToDigitPatterns(sLeft.Substring(1, 1), _aEvenLeft));
		stringBuilder.Append(ConvertToDigitPatterns(sLeft.Substring(2, 1), _aEvenLeft));
		stringBuilder.Append(ConvertToDigitPatterns(sLeft.Substring(3, 1), _aEvenLeft));
		stringBuilder.Append(ConvertToDigitPatterns(sLeft.Substring(4, 1), _aOddLeft));
		stringBuilder.Append(ConvertToDigitPatterns(sLeft.Substring(5, 1), _aOddLeft));
		return stringBuilder.ToString();
	}

	private string CountryCode7(string sLeft)
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(ConvertToDigitPatterns(sLeft.Substring(0, 1), _aOddLeft));
		stringBuilder.Append(ConvertToDigitPatterns(sLeft.Substring(1, 1), _aEvenLeft));
		stringBuilder.Append(ConvertToDigitPatterns(sLeft.Substring(2, 1), _aOddLeft));
		stringBuilder.Append(ConvertToDigitPatterns(sLeft.Substring(3, 1), _aEvenLeft));
		stringBuilder.Append(ConvertToDigitPatterns(sLeft.Substring(4, 1), _aOddLeft));
		stringBuilder.Append(ConvertToDigitPatterns(sLeft.Substring(5, 1), _aEvenLeft));
		return stringBuilder.ToString();
	}

	private string CountryCode8(string sLeft)
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(ConvertToDigitPatterns(sLeft.Substring(0, 1), _aOddLeft));
		stringBuilder.Append(ConvertToDigitPatterns(sLeft.Substring(1, 1), _aEvenLeft));
		stringBuilder.Append(ConvertToDigitPatterns(sLeft.Substring(2, 1), _aOddLeft));
		stringBuilder.Append(ConvertToDigitPatterns(sLeft.Substring(3, 1), _aEvenLeft));
		stringBuilder.Append(ConvertToDigitPatterns(sLeft.Substring(4, 1), _aEvenLeft));
		stringBuilder.Append(ConvertToDigitPatterns(sLeft.Substring(5, 1), _aOddLeft));
		return stringBuilder.ToString();
	}

	private string CountryCode9(string sLeft)
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(ConvertToDigitPatterns(sLeft.Substring(0, 1), _aOddLeft));
		stringBuilder.Append(ConvertToDigitPatterns(sLeft.Substring(1, 1), _aEvenLeft));
		stringBuilder.Append(ConvertToDigitPatterns(sLeft.Substring(2, 1), _aEvenLeft));
		stringBuilder.Append(ConvertToDigitPatterns(sLeft.Substring(3, 1), _aOddLeft));
		stringBuilder.Append(ConvertToDigitPatterns(sLeft.Substring(4, 1), _aEvenLeft));
		stringBuilder.Append(ConvertToDigitPatterns(sLeft.Substring(5, 1), _aOddLeft));
		return stringBuilder.ToString();
	}

	private string ConvertToDigitPatterns(string inputNumber, string[] patterns)
	{
		StringBuilder stringBuilder = new StringBuilder();
		int num = 0;
		for (int i = 0; i < inputNumber.Length; i++)
		{
			num = Convert.ToInt32(inputNumber.Substring(i, 1));
			stringBuilder.Append(patterns[num]);
		}
		return stringBuilder.ToString();
	}

	public void CalculateChecksumDigit()
	{
		string text = CountryCode + ManufacturerCode + ProductCode;
		int num = 0;
		int num2 = 0;
		for (int num3 = text.Length; num3 >= 1; num3--)
		{
			num2 = Convert.ToInt32(text.Substring(num3 - 1, 1));
			num = ((num3 % 2 != 0) ? (num + num2) : (num + num2 * 3));
		}
		ChecksumDigit = ((10 - num % 10) % 10).ToString();
	}
}
