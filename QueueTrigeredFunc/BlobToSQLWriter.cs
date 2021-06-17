using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace QueueTrigeredFunc
{
    public class BlobToSQLWriter
    {
        private readonly string _connString;
        private readonly BlobReader _reader;


        public BlobToSQLWriter(string connectionString, BlobReader reader) 
        {
            _connString = connectionString;
            _reader = reader;
        }

        public async Task WriteFileFRomBlobToDb(DocumentInfo documentInfo)
        {

            using (SqlConnection connection = new SqlConnection())
            {
                connection.ConnectionString = _connString;

                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandText = "INSERT INTO [Production].[Document]([DocumentNode],[Title],[Owner],[FolderFlag],[FileName],[FileExtension],[Revision],[ChangeNumber],[Status],[DocumentSummary],[Document],[rowguid],[ModifiedDate]) VALUES (@DocumentNode,@Title,@Owner,@FolderFlag,@FileName,@FileExtension,@Revision,@ChangeNumber,@Status,@DocumentSummary,@Document,@rowguid,@ModifiedDate)";
                    command.CommandType = CommandType.Text;
                    command.Parameters.Add("@DocumentNode", SqlDbType.NVarChar).Value = documentInfo.DocumentNode;
                    command.Parameters.Add("@Title", SqlDbType.NVarChar).Value = documentInfo.Title;
                    command.Parameters.Add("@Owner", SqlDbType.Int).Value = Int32.Parse(documentInfo.Owner);
                    command.Parameters.Add("FolderFlag", SqlDbType.Bit).Value = false;
                    command.Parameters.Add("@FileName", SqlDbType.NVarChar).Value = documentInfo.FileName;
                    command.Parameters.Add("@FileExtension", SqlDbType.NVarChar).Value = documentInfo.FileExtension;
                    command.Parameters.Add("@Revision", SqlDbType.NChar).Value = documentInfo.Revision;
                    command.Parameters.Add("@ChangeNumber", SqlDbType.Int).Value = Int32.Parse(documentInfo.ChangeNumber);
                    command.Parameters.Add("@Status", SqlDbType.TinyInt).Value = Byte.Parse(documentInfo.Status);
                    command.Parameters.Add("@DocumentSummary", SqlDbType.NVarChar).Value = documentInfo.DocumentSummary;
                    command.Parameters.Add("@Document", SqlDbType.VarBinary).Value = await _reader.GetBlobDocument(documentInfo.BlobFileName);
                    command.Parameters.Add("@rowguid", SqlDbType.UniqueIdentifier).Value = Guid.NewGuid();
                    command.Parameters.Add("@ModifiedDate", SqlDbType.DateTime).Value = DateTime.Now;
                    try
                    {
                        connection.Open();
                        Console.WriteLine("State: {0}", connection.State);
                        command.ExecuteNonQuery();

                    }
                    catch (SqlException e)
                    {
                        Console.WriteLine(e.Message.ToString(), "Error Message");
                    }
                }
            }
        }

    }
}
