namespace Corona.Tests
{
  // Parsing of the start-acquisition payload as Helios serializes an IAPI workflow.
  [TestClass]
  public class AcquisitionRequestTests
  {
    [TestMethod]
    public void Parse_PermanentAcquisition_ReadsEveryCommonField()
    {
      var request = AcquisitionRequest.Parse(
        "Kind=Permanent\nRawFileName=probe.raw\nSampleName=BSA\nComment=a comment; with = signs\nContinuation=Standby\nSingleProcessingDelay=2.5\nWaitForContactClosure=True\n");

      Assert.AreEqual(AcquisitionKind.Permanent, request.Kind);
      Assert.AreEqual("probe.raw", request.RawFileName);
      Assert.AreEqual("BSA", request.SampleName);
      Assert.AreEqual("a comment; with = signs", request.Comment);
      Assert.AreEqual(AcquisitionContinuation.Standby, request.Continuation);
      Assert.AreEqual(2.5, request.SingleProcessingDelay);
      Assert.IsTrue(request.WaitForContactClosure);
      Assert.IsNull(request.Duration);
    }

    [TestMethod]
    public void Parse_LimitedByCount_ReadsTheScanCount()
    {
      var request = AcquisitionRequest.Parse("Kind=LimitedByCount\nRawFileName=five.raw\nContinuation=StayOn\nNumberOfScans=5\n");
      Assert.AreEqual(AcquisitionKind.LimitedByCount, request.Kind);
      Assert.AreEqual(5, request.NumberOfScans);
    }

    [TestMethod]
    public void Parse_LimitedByTime_ReadsTheDurationInTimeSpanForm()
    {
      var request = AcquisitionRequest.Parse("Kind=LimitedByTime\nDuration=00:05:00\n");
      Assert.AreEqual(TimeSpan.FromMinutes(5), request.Duration);
    }

    [TestMethod]
    public void Parse_MethodRun_ReadsTheMethodAndAnOptionalDuration()
    {
      var request = AcquisitionRequest.Parse("Kind=MethodRun\nMethodName=C:\\Methods\\dda.meth\nDuration=01:30:00\n");
      Assert.AreEqual(AcquisitionKind.MethodRun, request.Kind);
      Assert.AreEqual("C:\\Methods\\dda.meth", request.MethodName);
      Assert.AreEqual(TimeSpan.FromMinutes(90), request.Duration);
    }

    [TestMethod]
    public void Parse_DefaultsWhatIsLeftOut_AndIgnoresUnknownKeys()
    {
      var request = AcquisitionRequest.Parse("Kind=Permanent\nFutureKey=whatever\n\nno equals sign here\n");
      Assert.AreEqual("", request.RawFileName);
      Assert.AreEqual(AcquisitionContinuation.StayOn, request.Continuation);
      Assert.AreEqual(0, request.SingleProcessingDelay);
      Assert.IsFalse(request.WaitForContactClosure);
    }

    [TestMethod]
    public void Parse_MissingOrUnknownKind_Throws()
    {
      Assert.ThrowsException<FormatException>(() => AcquisitionRequest.Parse("RawFileName=x.raw\n"));
      Assert.ThrowsException<FormatException>(() => AcquisitionRequest.Parse("Kind=Sequence\n"));
    }

    [TestMethod]
    public void Parse_UnreadableNumber_Throws()
    {
      Assert.ThrowsException<FormatException>(() => AcquisitionRequest.Parse("Kind=LimitedByCount\nNumberOfScans=five\n"));
      Assert.ThrowsException<FormatException>(() => AcquisitionRequest.Parse("Kind=LimitedByTime\nDuration=5 minutes\n"));
    }

    [TestMethod]
    public void ModeRequest_AcceptsOnlyTheModesTheInstrumentCanBePutIn()
    {
      Assert.IsTrue(ModeRequest.TryParse("Mode=On", out SystemMode on));
      Assert.AreEqual(SystemMode.On, on);
      Assert.IsTrue(ModeRequest.TryParse("mode = off\n", out SystemMode off));
      Assert.AreEqual(SystemMode.Off, off);
      Assert.IsFalse(ModeRequest.TryParse("Mode=DirectControl", out _));
      Assert.IsFalse(ModeRequest.TryParse("Mode=Warp", out _));
      Assert.IsFalse(ModeRequest.TryParse("", out _));
    }

    [TestMethod]
    public void KeyValueLines_SplitAtTheFirstEqualsAndTrim()
    {
      var values = KeyValueLines.Parse(" Key = a=b \r\nOther=\nnothing\n");
      Assert.AreEqual("a=b", values["Key"]);
      Assert.AreEqual("", values["other"]);
      Assert.AreEqual(2, values.Count);
    }
  }
}
