using Microsoft.VisualStudio.TestTools.UnitTesting;
using BugApp;
using System;

namespace BugTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestInitial() {
            Bug b = new Bug();
            Assert.AreEqual(Status.New, b.CurrentStatus);
        }

        [TestMethod]
        public void TestToAnalysis() {
            Bug b = new Bug();
            b.ApplyAction(BugTrigger.ToAnalysis);
            Assert.AreEqual(Status.Analyzing, b.CurrentStatus);
        }

        [TestMethod]
        public void TestPostpone() {
            Bug b = new Bug();
            b.ApplyAction(BugTrigger.ToAnalysis);
            b.ApplyAction(BugTrigger.Postpone);
            Assert.AreEqual(Status.Analyzing, b.CurrentStatus);
        }

        [TestMethod]
        public void TestReject() {
            Bug b = new Bug();
            b.ApplyAction(BugTrigger.ToAnalysis);
            b.ApplyAction(BugTrigger.Reject);
            Assert.AreEqual(Status.Reverted, b.CurrentStatus);
        }

        [TestMethod]
        public void TestHappyPath() {
            Bug b = new Bug();
            b.ApplyAction(BugTrigger.ToAnalysis);
            b.ApplyAction(BugTrigger.ToFix);
            b.ApplyAction(BugTrigger.ToVerify);
            b.ApplyAction(BugTrigger.FixOk);
            Assert.AreEqual(Status.Closed, b.CurrentStatus);
        }

        [TestMethod]
        public void TestFixFailed() {
            Bug b = new Bug();
            b.ApplyAction(BugTrigger.ToAnalysis);
            b.ApplyAction(BugTrigger.ToFix);
            b.ApplyAction(BugTrigger.ToVerify);
            b.ApplyAction(BugTrigger.FixFailed);
            Assert.AreEqual(Status.Reverted, b.CurrentStatus);
        }

        [TestMethod]
        public void TestCantRepro() {
            Bug b = new Bug();
            b.ApplyAction(BugTrigger.ToAnalysis);
            b.ApplyAction(BugTrigger.ToFix);
            b.ApplyAction(BugTrigger.CantReproduce);
            Assert.AreEqual(Status.Reverted, b.CurrentStatus);
        }

        [TestMethod]
        public void TestReopenFromReverted() {
            Bug b = new Bug();
            b.ApplyAction(BugTrigger.ToAnalysis);
            b.ApplyAction(BugTrigger.Reject);
            b.ApplyAction(BugTrigger.Reopen);
            Assert.AreEqual(Status.Analyzing, b.CurrentStatus);
        }

        [TestMethod]
        public void TestCloseFromReverted() {
            Bug b = new Bug();
            b.ApplyAction(BugTrigger.ToAnalysis);
            b.ApplyAction(BugTrigger.Reject);
            b.ApplyAction(BugTrigger.Close);
            Assert.AreEqual(Status.Closed, b.CurrentStatus);
        }

        [TestMethod]
        public void TestReopenFromClosed() {
            Bug b = new Bug();
            b.ApplyAction(BugTrigger.ToAnalysis);
            b.ApplyAction(BugTrigger.ToFix);
            b.ApplyAction(BugTrigger.ToVerify);
            b.ApplyAction(BugTrigger.FixOk);
            b.ApplyAction(BugTrigger.Reopen);
            Assert.AreEqual(Status.Analyzing, b.CurrentStatus);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestInvalidTransition() {
            Bug b = new Bug();
            b.ApplyAction(BugTrigger.ToFix);
        }
    }
}
