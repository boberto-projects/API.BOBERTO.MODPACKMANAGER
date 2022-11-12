namespace MinecraftServer.Api.Models
{
    public class TransactionModel
    {
        /// <summary>
        /// 12/11/2022 20:43
        /// Todo: Necessário adicionar Transaction em todos os serviços que alterem dados em MongoService
        /// Atualmente apenas o UpdateSet foi atualizado.
        /// Todo: Verificar o uso de memória e o tempo do request.
        /// </summary>
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
