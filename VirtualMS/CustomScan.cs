using System;
using System.Collections.Generic;

namespace VirtualMS
{
  public class CustomScan : EventArgs
  {
    public IDictionary<string, string> Values { get; set; }
    public double RetentionTime = 0;

    public CustomScan()
    {
      Values = new Dictionary<string, string>();
    }

    public void Deserialize(byte[] data)
    {
      using (MemoryStream m = new MemoryStream(data))
      using (BinaryReader reader = new BinaryReader(m, System.Text.Encoding.Unicode))
      {
        int count = reader.ReadInt32();
        for (int a = 0; a < count; a++)
        {
          string key = reader.ReadString();
          string value = reader.ReadString();
          Values.Add(key, value);
        }
      }
    }

  }
}
