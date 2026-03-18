/* * * * * * * * * * *
* Calico Rose
* Sources:
* QR Code Generation: https://www.thonky.com/qr-code-tutorial/introduction
* ISO/IEC 18004 Standard: https://raw.githubusercontent.com/yansikeim/QR-Code/master/ISO%20IEC%2018004%202015%20Standard.pdf
* github.com/codebude/QRCoder
* * * * * * * * * * */

using Godot;
using System;
using System.Collections.Generic;

/* * * * * * * * * * *
* Generates two QR codes from a row in the Google Sheet, looked up by the ID in column A.
* Requires SheetManager to be set up as an Autoload.
*
* SHEET STRUCTURE:
*   Column A  →  "ID"                 (the row identifier you search by)
*   Column F  →  "NAT_ORG"            (national organization title)
*   Column G  →  "NAT_ORG_DESCRIP"    (national organization description)
*   Column H  →  "NAT_ORG_URL"        (first QR code URL)
*   Column I  →  "LOCAL_ORG"          (local organization title)
*   Column J  →  "LOCAL_ORG_DESCRIP"  (local organization description)
*   Column K  →  "LOCAL_ORG_URL"      (second QR code URL)
*
* Call GenerateQRsForID("your-id-value") from any other script.
* EXAMPLE:
*   GetNode<QRCodeGenerator>("QRNode").GenerateQRsForID("ORG-042");
* * * * * * * * * * */

public partial class QRCodeGenerator : Node
{
	[Export] public TextureRect NatOrgRect;   // displays NAT_ORG_URL  (column H)
	[Export] public TextureRect LocalOrgRect; // displays LOCAL_ORG_URL (column K)
	[Export] public RichTextLabel NatOrgName; // displays NAT_ORG (column F)
	[Export] public RichTextLabel NatOrgDescrip; // displays NAT_ORG_DESCRIP (column G)
	[Export] public RichTextLabel LocalOrgName; // displays LOCAL_ORG (column I)
	[Export] public RichTextLabel LocalOrgDescrip; // displays LOCAL_ORG_DESCRIP (column J)

	// Size of each QR "module" (pixel block). Increase for a larger image.
	[Export] public int ModuleSize = 8;

	private const string COL_ID = "ID";
	private const string COL_NAT_ORG = "NAT_ORG";
	private const string COL_NAT_ORG_DESCRIP = "NAT_ORG_DESCRIP";
	private const string COL_NAT_ORG_URL = "NAT_ORG_URL";
	private const string COL_LOCAL_ORG = "LOCAL_ORG";
	private const string COL_LOCAL_ORG_DESCRIP = "LOCAL_ORG_DESCRIP";
	private const string COL_LOCAL_ORG_URL = "LOCAL_ORG_URL";

	/* * * * * * * * * * *
	* Public API
	* * * * * * * * * * */

	/* * * * * * * * * * *
	* Looks up the row whose column A value matches the given ID, then
	* generates a QR code for NAT_ORG_URL and LOCAL_ORG_URL from that row.
	* Waits automatically if SheetManager hasn't finished loading yet.
	* * * * * * * * * * */
	
	public void GenerateQRsForID(string id) {
		if (!SheetManager.Instance.IsReady) {
			GD.Print("[QRCodeGenerator] Sheet not ready yet — waiting...");
			SheetManager.Instance.DataLoaded += () => GenerateQRsForID(id);
			SheetManager.Instance.DataFailed += (err) => GD.PrintErr($"[QRCodeGenerator] SheetManager failed: {err}");
			return;
		}

		int row = SheetManager.Instance.FindRow(COL_ID, id);
		if (row == -1) {
			GD.PrintErr($"[QRCodeGenerator] No row found where {COL_ID} = \"{id}\".");
			return;
		}

		string natUrl = SheetManager.Instance.GetCell(row, COL_NAT_ORG_URL);
		string localUrl = SheetManager.Instance.GetCell(row, COL_LOCAL_ORG_URL);
		string natName = SheetManager.Instance.GetCell(row, COL_NAT_ORG);
		string natDescrip = SheetManager.Instance.GetCell(row, COL_NAT_ORG_DESCRIP);
		string localName = SheetManager.Instance.GetCell(row, COL_LOCAL_ORG);
		string localDescrip = SheetManager.Instance.GetCell(row, COL_LOCAL_ORG_DESCRIP);

		GD.Print($"[QRCodeGenerator] Row {row} | {COL_NAT_ORG_URL}: {natUrl}");
		GD.Print($"[QRCodeGenerator] Row {row} | {COL_LOCAL_ORG_URL}: {localUrl}");

		if (string.IsNullOrEmpty(natUrl)) {
			GD.PrintErr($"[QRCodeGenerator] {COL_NAT_ORG_URL} is empty for ID \"{id}\" — skipping.");
		} else {
			SetTexture(NatOrgRect, natUrl, nameof(NatOrgRect));
		}

		if (string.IsNullOrEmpty(localUrl)) {
			GD.PrintErr($"[QRCodeGenerator] {COL_LOCAL_ORG_URL} is empty for ID \"{id}\" — skipping.");
		} else {
			SetTexture(LocalOrgRect, localUrl, nameof(LocalOrgRect));
		}
		
		if(string.IsNullOrEmpty(natName)) {
			GD.PrintErr($"[QRCodeGenerator] {COL_NAT_ORG} is empty for ID \"{id}\" — skipping.");
		} else {
			NatOrgName.Text = natName;
		}
		
		if(string.IsNullOrEmpty(natDescrip)) {
			GD.PrintErr($"[QRCodeGenerator] {COL_NAT_ORG_DESCRIP} is empty for ID \"{id}\" — skipping.");
		} else {
			NatOrgDescrip.Text = natDescrip;
		}
		
		if(string.IsNullOrEmpty(localName)) {
			GD.PrintErr($"[QRCodeGenerator] {COL_LOCAL_ORG} is empty for ID \"{id}\" — skipping.");
		} else {
			LocalOrgName.Text = localName;
			
		}
		
		if(string.IsNullOrEmpty(localDescrip)) {
			GD.PrintErr($"[QRCodeGenerator] {COL_LOCAL_ORG_DESCRIP} is empty for ID \"{id}\" — skipping.");
		} else {
			LocalOrgDescrip.Text = localDescrip;
		}
	}

	/* * * * * * * * * * *
	* Skips the sheet lookup and generates both QR codes directly from strings.
	* Useful for testing without needing a live sheet.
	* * * * * * * * * * */
	public void GenerateQRsDirect(string natUrl, string localUrl) {
		if (!string.IsNullOrEmpty(natUrl)) {
			SetTexture(NatOrgRect, natUrl, nameof(NatOrgRect));
		}
		if (!string.IsNullOrEmpty(localUrl)) {
			SetTexture(LocalOrgRect, localUrl, nameof(LocalOrgRect));
		}
	}

	/* * * * * * * * * * *
	* Internal helpers
	* * * * * * * * * * */

	private void SetTexture(TextureRect rect, string text, string rectName) {
		if (rect == null) {
			GD.PrintErr($"[QRCodeGenerator] {rectName} is not assigned in the Inspector.");
			return;
		}
		try {
			bool[,] modules = QREncoder.Encode(text);
			rect.Texture    = RenderToTexture(modules);
		} catch (Exception e) {
			GD.PrintErr($"[QRCodeGenerator] Failed to generate QR for {rectName}: {e.Message}");
		}
	}

	private ImageTexture RenderToTexture(bool[,] modules) {
		int size      = modules.GetLength(0);
		int quietZone = 4; // mandatory white border (in modules) per QR spec
		int px        = (size + quietZone * 2) * ModuleSize;

		var image = Image.CreateEmpty(px, px, false, Image.Format.Rgb8);
		image.Fill(Colors.White);

		for (int row = 0; row < size; row++) {
			for (int col = 0; col < size; col++) {
				if (!modules[row, col]) {
					continue;
				}
				int x = (col + quietZone) * ModuleSize;
				int y = (row + quietZone) * ModuleSize;
				for (int dy = 0; dy < ModuleSize; dy++) {
					for (int dx = 0; dx < ModuleSize; dx++) {
						image.SetPixel(x + dx, y + dy, Colors.Black);
					}
				}
			}
		}

		return ImageTexture.CreateFromImage(image);
	}
}

/* * * * * * * * * * * *
* QR ENCODER — Version 1–4, byte mode, ECC level M. No external libs.
* Supports URLs up to 62 bytes. For longer URLs, shorten them first or
* integrate the QRCoder .dll for full version support.
* * * * * * * * * * * */

internal static class QREncoder
{
	public static bool[,] Encode(string text) {
		byte[] data      = System.Text.Encoding.UTF8.GetBytes(text);
		int version      = PickVersion(data.Length);
		byte[] codewords = BuildCodewords(data, version);
		byte[] withECC   = AddErrorCorrection(codewords, version);
		bool[,] matrix   = BuildMatrix(version);
		PlaceData(matrix, withECC, version);
		int mask         = ApplyBestMask(matrix, version);
		DrawFormatInfo(matrix, mask, version);
		return matrix;
	}

	// Max data bytes per version (ECC level M, byte mode)
	private static readonly int[] MaxBytes = { 0, 14, 26, 42, 62 };

	private static int PickVersion(int byteCount) {
		for (int v = 1; v <= 4; v++) {
			if (byteCount <= MaxBytes[v]) {
				return v;
			}
		}
		throw new Exception(
			$"URL is {byteCount} bytes — too long for the built-in encoder (max 62 bytes). " +
			"Please shorten the URL or integrate the QRCoder library for full version support."
		);
	}

	// Codeword builder

	private static readonly int[] TotalDC = { 0, 16, 28, 44, 64 };

	private static byte[] BuildCodewords(byte[] data, int version) {
		int capacity = TotalDC[version];
		var bits     = new BitWriter();

		bits.Write(0b0100, 4);      // mode indicator: byte mode
		bits.Write(data.Length, 8); // character count (8 bits for versions 1-9)
		foreach (byte b in data) bits.Write(b, 8);
		bits.Write(0, Math.Min(4, capacity * 8 - bits.Length)); // terminator
		while (bits.Length % 8 != 0) bits.Write(0, 1);          // pad to byte boundary

		bool toggle = true;
		while (bits.Length < capacity * 8) {
			bits.Write(toggle ? 0b11101100 : 0b00010001, 8);
			toggle = !toggle;
		}
		return bits.ToBytes();
	}

	// Reed-Solomon ECC

	private static readonly int[] ECCPerBlock = { 0, 10, 16, 26, 18 };
	private static readonly int[] NumBlocks   = { 0,  1,  1,  1,  2 };

	private static byte[] AddErrorCorrection(byte[] data, int version) {
		int eccCount  = ECCPerBlock[version];
		int numBlocks = NumBlocks[version];
		int blockSize = data.Length / numBlocks;

		var result   = new List<byte>();
		var ecBlocks = new List<byte[]>();

		for (int b = 0; b < numBlocks; b++) {
			byte[] block = new byte[blockSize];
			Array.Copy(data, b * blockSize, block, 0, blockSize);
			result.AddRange(block);
			ecBlocks.Add(ReedSolomon(block, eccCount));
		}
		foreach (var ec in ecBlocks) {
			result.AddRange(ec);
		}
		return result.ToArray();
	}

	private static byte[] ReedSolomon(byte[] data, int eccCount) {
		byte[] gen = RSGenerator(eccCount);
		byte[] msg = new byte[data.Length + eccCount];
		Array.Copy(data, msg, data.Length);

		for (int i = 0; i < data.Length; i++) {
			byte coef = msg[i];
			if (coef == 0) {
				continue;
			}
			for (int j = 0; j < gen.Length; j++) {
				msg[i + j] ^= GFMul(gen[j], coef);
			}
		}

		byte[] ecc = new byte[eccCount];
		Array.Copy(msg, data.Length, ecc, 0, eccCount);
		return ecc;
	}

	private static byte[] RSGenerator(int degree) {
		byte[] g = { 1 };
		for (int i = 0; i < degree; i++) {
			byte[] next = new byte[g.Length + 1];
			byte root   = GFExpTable[i % 255];
			for (int j = 0; j < g.Length; j++) {
				next[j]     ^= g[j];
				next[j + 1] ^= GFMul(g[j], root);
			}
			g = next;
		}
		return g;
	}

	// Galois Field GF(256)
	private static readonly byte[] GFExpTable = BuildExpTable();
	private static readonly byte[] GFLogTable = BuildLogTable();

	private static byte[] BuildExpTable() {
		var t = new byte[256];
		int x = 1;
		for (int i = 0; i < 255; i++) {
			t[i] = (byte)x;
			x <<= 1;
			if (x >= 256) {
				x ^= 0x11D;
			}
		}
		t[255] = t[0];
		return t;
	}

	private static byte[] BuildLogTable() {
		var t = new byte[256];
		for (int i = 0; i < 255; i++) {
			t[GFExpTable[i]] = (byte)i;
		}
		return t;
	}

	private static byte GFMul(byte a, byte b) {
		if (a == 0 || b == 0) {
			return 0;
		}
		return GFExpTable[(GFLogTable[a] + GFLogTable[b]) % 255];
	}

	// Matrix construction

	private static int MatrixSize(int version) => version * 4 + 17;

	private static bool[,] BuildMatrix(int version) {
		int size    = MatrixSize(version);
		var matrix  = new bool[size, size];
		var funcMap = new bool[size, size];
		DrawFinderPatterns(matrix, funcMap, size);
		DrawTimingPatterns(matrix, funcMap, size);
		if (version >= 2) DrawAlignmentPattern(matrix, funcMap, version);
		ReserveFormatArea(matrix, funcMap, size);
		return matrix;
	}

	private static void DrawFinderPatterns(bool[,] m, bool[,] f, int size) {
		int[][] origins = { new[]{0,0}, new[]{0, size-7}, new[]{size-7, 0} };
		foreach (var o in origins) {
			DrawFinder(m, f, o[0], o[1]);
			for (int i = -1; i <= 7; i++) {
				for (int j = -1; j <= 7; j++) {
					int r = o[0]+i, c = o[1]+j;
					if (r >= 0 && r < size && c >= 0 && c < size) {
						f[r, c] = true;
					}
				}
			}
		}
	}

	private static void DrawFinder(bool[,] m, bool[,] f, int row, int col) {
		bool[,] p = {
			{true,true,true,true,true,true,true},
			{true,false,false,false,false,false,true},
			{true,false,true,true,true,false,true},
			{true,false,true,true,true,false,true},
			{true,false,true,true,true,false,true},
			{true,false,false,false,false,false,true},
			{true,true,true,true,true,true,true}
		};
		for (int r = 0; r < 7; r++) {
			for (int c = 0; c < 7; c++) {
				m[row+r, col+c] = p[r, c];
				f[row+r, col+c] = true;
			}
		}
	}

	private static void DrawTimingPatterns(bool[,] m, bool[,] f, int size) {
		for (int i = 8; i < size-8; i++) {
			m[6, i] = (i % 2 == 0); f[6, i] = true;
			m[i, 6] = (i % 2 == 0); f[i, 6] = true;
		}
	}

	private static void DrawAlignmentPattern(bool[,] m, bool[,] f, int version) {
		int[] centers = { 0, 0, 18, 22, 26 };
		int c = centers[version];
		bool[,] p = {
			{true,true,true,true,true},
			{true,false,false,false,true},
			{true,false,true,false,true},
			{true,false,false,false,true},
			{true,true,true,true,true}
		};
		for (int r = -2; r <= 2; r++) {
			for (int cc = -2; cc <= 2; cc++) {
				m[c+r, c+cc] = p[r+2, cc+2];
				f[c+r, c+cc] = true;
			}
		}
	}

	private static void ReserveFormatArea(bool[,] m, bool[,] f, int size) {
		for (int i = 0; i < 9; i++) {
			f[8, i] = true; f[i, 8] = true;
		}
		for (int i = size-8; i < size; i++) {
			f[8, i] = true; f[i, 8] = true;
		}
		m[size-8, 8] = true; f[size-8, 8] = true; // dark module
	}

	//Data placement

	private static void PlaceData(bool[,] matrix, byte[] data, int version) {
		int size    = MatrixSize(version);
		var funcMap = RebuildFuncMap(version, size);

		var bits = new List<bool>();
		foreach (byte b in data) {
			for (int i = 7; i >= 0; i--) {
				bits.Add((b >> i & 1) == 1);
			}
		}

		int bitIdx  = 0;
		bool upward = true;

		for (int col = size-1; col >= 1; col -= 2) {
			if (col == 6) {
				col = 5;
			}
			for (int rowOffset = 0; rowOffset < size; rowOffset++) {
				int row = upward ? size-1-rowOffset : rowOffset;
				for (int dx = 0; dx <= 1; dx++) {
					int c = col - dx;
					if (!funcMap[row, c]) {
						matrix[row, c] = bitIdx < bits.Count && bits[bitIdx];
						bitIdx++;
					}
				}
			}
			upward = !upward;
		}
	}

	private static bool[,] RebuildFuncMap(int version, int size) {
		var f = new bool[size, size];
		var d = new bool[size, size];
		DrawFinderPatterns(d, f, size);
		DrawTimingPatterns(d, f, size);
		if (version >= 2) {
			DrawAlignmentPattern(d, f, version);
		}
		ReserveFormatArea(d, f, size);
		return f;
	}

	// Masking

	private static int ApplyBestMask(bool[,] matrix, int version) {
		int size     = MatrixSize(version);
		var funcMap  = RebuildFuncMap(version, size);
		int bestMask = 0, bestScore = int.MaxValue;

		for (int mask = 0; mask < 8; mask++) {
			var candidate = (bool[,])matrix.Clone();
			ApplyMask(candidate, funcMap, size, mask);
			DrawFormatInfo(candidate, mask, version);
			int score = PenaltyScore(candidate, size);
			if (score < bestScore) {
				bestScore = score; bestMask = mask;
			}
		}

		ApplyMask(matrix, funcMap, size, bestMask);
		return bestMask;
	}

	private static void ApplyMask(bool[,] m, bool[,] f, int size, int mask) {
		for (int r = 0; r < size; r++) {
			for (int c = 0; c < size; c++) {
				if (f[r, c]) {
					continue;
				}
				bool flip = mask switch {
					0 => (r + c) % 2 == 0,
					1 => r % 2 == 0,
					2 => c % 3 == 0,
					3 => (r + c) % 3 == 0,
					4 => (r / 2 + c / 3) % 2 == 0,
					5 => (r * c) % 2 + (r * c) % 3 == 0,
					6 => ((r * c) % 2 + (r * c) % 3) % 2 == 0,
					7 => ((r + c) % 2 + (r * c) % 3) % 2 == 0,
					_ => false
				};
				if (flip) {
					m[r, c] = !m[r, c];
				}
			}
		}
	}

	// Format strings for ECC level M, masks 0–7
	private static readonly int[] FormatBits = {
		0x5412, 0x5125, 0x5E7C, 0x5B4B, 0x45F9, 0x40CE, 0x4F97, 0x4AA0
	};

	private static void DrawFormatInfo(bool[,] m, int mask, int version) {
		int size = MatrixSize(version);
		int fmt  = FormatBits[mask];

		for (int i = 0; i < 6; i++) {
			bool bit = (fmt >> (14-i) & 1) == 1;
			m[8, i] = bit; m[i, 8] = bit;
		}
		m[8, 7] = (fmt >> 8 & 1) == 1; m[7, 8] = (fmt >> 8 & 1) == 1;
		m[8, 8] = (fmt >> 7 & 1) == 1;
		for (int i = 9; i < 15; i++) {
			bool bit = (fmt >> (14-i) & 1) == 1;
			m[8, size-15+i] = bit;
			m[size-15+i, 8] = bit;
		}
	}

	// Penalty scoring

	private static int PenaltyScore(bool[,] m, int size) {
		int score = 0;

		// Rule 1: runs of 5+ same colour in a row or column
		for (int r = 0; r < size; r++) {
			int run = 1;
			for (int c = 1; c < size; c++) {
				if (m[r,c] == m[r,c-1]) {
					run++;
				} else {
					if (run >= 5) {
						score += run-2; run = 1;
					}
				}
			}
			if (run >= 5) {
				score += run-2;
			}
		}
		for (int c = 0; c < size; c++) {
			int run = 1;
			for (int r = 1; r < size; r++) {
				if (m[r,c] == m[r-1,c]) {
					run++;
				} else {
					if (run >= 5) {
						score += run-2; 
						run = 1; 
					}
				}
			}
			if (run >= 5) {
				score += run-2;
			}
		}

		// Rule 2: 2x2 blocks of the same colour
		for (int r = 0; r < size-1; r++) {
			for (int c = 0; c < size-1; c++) {
				if (m[r,c]==m[r,c+1] && m[r,c]==m[r+1,c] && m[r,c]==m[r+1,c+1]) {
					score += 3;
				}
			}
		}

		return score;
	}

	// Bit writer

	private class BitWriter {
		private readonly List<bool> _bits = new();
		public int Length => _bits.Count;

		public void Write(int value, int numBits) {
			for (int i = numBits-1; i >= 0; i--) {
				_bits.Add((value >> i & 1) == 1);
			}
		}

		public byte[] ToBytes() {
			var bytes = new byte[(_bits.Count + 7) / 8];
			for (int i = 0; i < _bits.Count; i++) {
				if (_bits[i]) {
					bytes[i/8] |= (byte)(1 << (7 - i%8));
				}
			}
			return bytes;
		}
	}
}
