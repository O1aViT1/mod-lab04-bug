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

    public enum Action
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
        private StateMachine<Status, Action> stateMachine;

        public Status CurrentStatus => stateMachine.State;

        public Bug()
        {
            stateMachine = new StateMachine<Status, Action>(Status.New);

            stateMachine.Configure(Status.New)
                .Permit(Action.ToAnalysis, Status.Analyzing);

            stateMachine.Configure(Status.Analyzing)
                .PermitReentry(Action.Postpone) // Остается в том же статусе
                .Permit(Action.Reject, Status.Reverted)
                .Permit(Action.ToFix, Status.Fixing);

            stateMachine.Configure(Status.Fixing)
                .Permit(Action.CantReproduce, Status.Reverted)
                .Permit(Action.ToVerify, Status.Verification);

            stateMachine.Configure(Status.Verification)
                .Permit(Action.FixOk, Status.Closed)
                .Permit(Action.FixFailed, Status.Reverted);

            stateMachine.Configure(Status.Reverted)
                .Permit(Action.Close, Status.Closed)
                .Permit(Action.Reopen, Status.Analyzing);

            stateMachine.Configure(Status.Closed)
                .Permit(Action.Reopen, Status.Analyzing);
        }

        public void ApplyAction(Action action)
        {
            stateMachine.Fire(action);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("--- Тестирование автомата багов ---");
            
            Bug myBug = new Bug();
            Console.WriteLine("Создан: " + myBug.CurrentStatus);

            myBug.ApplyAction(Action.ToAnalysis);
            Console.WriteLine("Взят в разбор: " + myBug.CurrentStatus);

            myBug.ApplyAction(Action.ToFix);
            Console.WriteLine("Взят в работу (исправление): " + myBug.CurrentStatus);

            myBug.ApplyAction(Action.ToVerify);
            Console.WriteLine("Готов к проверке: " + myBug.CurrentStatus);

            myBug.ApplyAction(Action.FixOk);
            Console.WriteLine("Проверка пройдена, статус: " + myBug.CurrentStatus);
        }
    }
}
