namespace MinecraftServer.Api.Models
{
    public class TransactionModel
    {
        private bool isTransaction { get; set; }
        private List<Task> Changes { get; set; }

        public void StartTransaction()
        {
            this.isTransaction = true;
            this.Changes = new List<Task>();
        }
        public void Commit(Task task)
        {
            Changes.Add(task);
        }
        public bool SaveChanges()
        {
            Task t = Task.WhenAll(Changes);
            try
            {
                t.Wait();
            }
            catch { }

            if (t.Status == TaskStatus.Faulted)
                return false;

            this.Abort();

            return true;
        }

        public bool IsTransaction()
        {
            return this.isTransaction;
        }

        public void Abort()
        {
            this.isTransaction = false;
            this.Changes.Clear();
        }

    }
}
