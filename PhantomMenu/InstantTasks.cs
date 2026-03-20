namespace PhantomMenu
{
    public static class InstantTasks
    {
        public static void CompleteAll()
        {
            var localPlayer = PlayerControl.LocalPlayer;
            if (localPlayer == null) return;

            foreach (var task in localPlayer.myTasks)
            {
                if (task == null) continue;
                var normalTask = task.TryCast<NormalPlayerTask>();
                if (normalTask == null || normalTask.IsComplete) continue;
                localPlayer.RpcCompleteTask(normalTask.Id);
            }
        }
    }
}
