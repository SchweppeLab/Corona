using System.IO;
using System.Text;

namespace Corona.Tests
{
  // CustomScan.Deserialize reads the client's custom-scan request: an Int32 pair count, then
  // that many (key, value) string pairs, written by a Unicode BinaryWriter on the client side.
  [TestClass]
  public class CustomScanTests
  {
    internal static byte[] Encode(params (string Key, string Value)[] pairs)
    {
      using var stream = new MemoryStream();
      using (var writer = new BinaryWriter(stream, Encoding.Unicode, leaveOpen: true))
      {
        writer.Write(pairs.Length);
        foreach (var (key, value) in pairs)
        {
          writer.Write(key);
          writer.Write(value);
        }
      }
      return stream.ToArray();
    }

    [TestMethod]
    public void Deserialize_ReadsEveryPair()
    {
      var scan = new CustomScan();

      scan.Deserialize(Encode(("ScanType", "Full"), ("FirstMass", "400"), ("LastMass", "1600")));

      Assert.AreEqual(3, scan.Values.Count);
      Assert.AreEqual("Full", scan.Values["ScanType"]);
      Assert.AreEqual("400", scan.Values["FirstMass"]);
      Assert.AreEqual("1600", scan.Values["LastMass"]);
    }

    [TestMethod]
    public void Deserialize_EmptyRequest_HasNoValues()
    {
      var scan = new CustomScan();

      scan.Deserialize(Encode());

      Assert.AreEqual(0, scan.Values.Count);
    }

    [TestMethod]
    public void Deserialize_TruncatedRequest_Throws()
    {
      // The premise of the guard in VMSServer.OnCustomScanRequest: a cut-off message must
      // throw here so the server can drop it, rather than yield a half-read request.
      byte[] whole = Encode(("ScanType", "Full"));
      byte[] truncated = whole[..^3];
      var scan = new CustomScan();

      Assert.ThrowsException<EndOfStreamException>(() => scan.Deserialize(truncated));
    }

    [TestMethod]
    public void Deserialize_ImpossiblePairCount_Throws()
    {
      byte[] garbage = { 0xFF, 0xFF, 0xFF, 0x7F, 0x41 };
      var scan = new CustomScan();

      Assert.ThrowsException<EndOfStreamException>(() => scan.Deserialize(garbage));
    }
  }
}
