using Microsoft.VisualStudio.TestTools.UnitTesting;
using BugApp;
using System;

namespace BugTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void CheckInitialStatusIsNew()
        {
            Bug b = new Bug();
            Assert.AreEqual(Status.New, b.CurrentStatus);
        }

        [TestMethod]
        public void ActionToAnalysisChangesStatus()
        {
            Bug b = new Bug();
            b.ApplyAction(Action.ToAnalysis);
            Assert.AreEqual(Status.Analyzing, b.CurrentStatus);
        }

        [TestMethod]
        public void ActionPostponeKeepsInAnalyzing()
        {
            Bug b = new Bug();
            b.ApplyAction(Action.ToAnalysis);
            b.ApplyAction(Action.Postpone);
            Assert.AreEqual(Status.Analyzing, b.CurrentStatus);
        }

        [TestMethod]
        public void ActionRejectMovesToReverted()
        {
            Bug b = new Bug();
            b.ApplyAction(Action.ToAnalysis);
            b.ApplyAction(Action.Reject);
            Assert.AreEqual(Status.Reverted, b.CurrentStatus);
        }

        [TestMethod]
        public void HappyPathToClosed()
        {
            Bug b = new Bug();
            b.ApplyAction(Action.ToAnalysis);
            b.ApplyAction(Action.ToFix);
            b.ApplyAction(Action.ToVerify);
            b.ApplyAction(Action.FixOk);
            Assert.AreEqual(Status.Closed, b.CurrentStatus);
        }

        [TestMethod]
        public void FixFailedMovesToReverted()
        {
            Bug b = new Bug();
            b.ApplyAction(Action.ToAnalysis);
            b.ApplyAction(Action.ToFix);
            b.ApplyAction(Action.ToVerify);
            b.ApplyAction(Action.FixFailed);
            Assert.AreEqual(Status.Reverted, b.CurrentStatus);
        }

        [TestMethod]
        public void CantReproduceMovesToReverted()
        {
            Bug b = new Bug();
            b.ApplyAction(Action.ToAnalysis);
            b.ApplyAction(Action.ToFix);
            b.ApplyAction(Action.CantReproduce);
            Assert.AreEqual(Status.Reverted, b.CurrentStatus);
        }

        [TestMethod]
        public void ReopenFromRevertedMovesToAnalyzing()
        {
            Bug b = new Bug();
            b.ApplyAction(Action.ToAnalysis);
            b.ApplyAction(Action.Reject);
            b.ApplyAction(Action.Reopen);
            Assert.AreEqual(Status.Analyzing, b.CurrentStatus);
        }

        [TestMethod]
        public void CloseFromRevertedMovesToClosed()
        {
            Bug b = new Bug();
            b.ApplyAction(Action.ToAnalysis);
            b.ApplyAction(Action.Reject);
            b.ApplyAction(Action.Close);
            Assert.AreEqual(Status.Closed, b.CurrentStatus);
        }

        [TestMethod]
        public void ReopenFromClosedMovesToAnalyzing()
        {
            Bug b = new Bug();
            b.ApplyAction(Action.ToAnalysis);
            b.ApplyAction(Action.ToFix);
            b.ApplyAction(Action.ToVerify);
            b.ApplyAction(Action.FixOk);
            b.ApplyAction(Action.Reopen);
            Assert.AreEqual(Status.Analyzing, b.CurrentStatus);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void WrongActionThrowsException()
        {
            Bug b = new Bug();
            b.ApplyAction(Action.ToFix);
        }
    }
}
