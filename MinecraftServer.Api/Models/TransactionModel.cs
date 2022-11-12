namespace MinecraftServer.Api.Models
{
    public class TransactionModel
    {
        private bool isTransaction { get; set; }
        private List<Task> Changes { get; set; }

        public void StartTransaction()
        {
            this.isTransaction = true;
        }
        public void Commit(Task task)
        {
            Changes.Add(task);
        }
        public void Finalize()
        {
            Task t = Task.WhenAll(Changes);
            try
            {
                t.Wait();
            }
            catch { }

            //if (t.Status == TaskStatus.RanToCompletion)
            //    Console.WriteLine("All ping attempts succeeded.");
            //else if (t.Status == TaskStatus.Faulted)
            //    Console.WriteLine("{0} ping attempts failed", failed);
        }

        public void Abort()
        {

        }

    }
}
