using System;
using Stateless;

namespace BugApp
{
    public enum Status
    {
        New, 
        Analyzing, 
        Fixing, 
        Verification, 
        Reverted, 
        Closed
    }

    public enum BugTrigger
    {
        ToAnalysis, 
        Postpone, 
        Reject, 
        ToFix, 
        CantReproduce, 
        ToVerify, 
        FixOk, 
        FixFailed, 
        Close, 
        Reopen
    }

    public class Bug
    {
        private StateMachine<Status, BugTrigger> stateMachine;

        public Status CurrentStatus => stateMachine.State;

        public Bug()
        {
            stateMachine = new StateMachine<Status, BugTrigger>(Status.New);

            stateMachine.Configure(Status.New)
                .Permit(BugTrigger.ToAnalysis, Status.Analyzing);

            stateMachine.Configure(Status.Analyzing)
                .PermitReentry(BugTrigger.Postpone) 
                .Permit(BugTrigger.Reject, Status.Reverted)
                .Permit(BugTrigger.ToFix, Status.Fixing);

            stateMachine.Configure(Status.Fixing)
                .Permit(BugTrigger.CantReproduce, Status.Reverted)
                .Permit(BugTrigger.ToVerify, Status.Verification);

            stateMachine.Configure(Status.Verification)
                .Permit(BugTrigger.FixOk, Status.Closed)
                .Permit(BugTrigger.FixFailed, Status.Reverted);

            stateMachine.Configure(Status.Reverted)
                .Permit(BugTrigger.Close, Status.Closed)
                .Permit(BugTrigger.Reopen, Status.Analyzing);

            stateMachine.Configure(Status.Closed)
                .Permit(BugTrigger.Reopen, Status.Analyzing);
        }

        public void ApplyAction(BugTrigger trigger)
        {
            stateMachine.Fire(trigger);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("--- Тестирование автомата багов ---");
            
            Bug myBug = new Bug();
            Console.WriteLine("Создан: " + myBug.CurrentStatus);

            myBug.ApplyAction(BugTrigger.ToAnalysis);
            Console.WriteLine("Взят в разбор: " + myBug.CurrentStatus);

            myBug.ApplyAction(BugTrigger.ToFix);
            Console.WriteLine("Взят в работу (исправление): " + myBug.CurrentStatus);

            myBug.ApplyAction(BugTrigger.ToVerify);
            Console.WriteLine("Готов к проверке: " + myBug.CurrentStatus);

            myBug.ApplyAction(BugTrigger.FixOk);
            Console.WriteLine("Проверка пройдена, статус: " + myBug.CurrentStatus);
        }
    }
}
