using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Orchard.ContentManagement;
using Orchard.Environment.Commands;
using Orchard.Tests.Stubs;
using Xunit;
using YesSql;
using YesSql.Indexes;
using YesSql.Provider.SqlServer;
using YesSql.Sql;

namespace Orchard.Tests.Data
{
    public class MyPage
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public DateTime SomeDate { get; set; }
    }

    public class MyPageByYear : MapIndex
    {
        public string Title { get; set; }
        public string Body { get; set; }
        public int Year { get; set; }
    }

    public class MyPageIndexProvider : IndexProvider<MyPage>
    {
        public override void Describe(DescribeContext<MyPage> context)
        {
            context.For<MyPageByYear>()
                .Map(page => new MyPageByYear { Title = page.Title, Body = page.Body, Year = page.SomeDate.Year });
        }
    }

    public class YesSqlTests : IDisposable
    {
        private Store _store;

        public YesSqlTests()
        {

            string connectionString = @"Data Source = (localdb)\mssqllocaldb; Initial Catalog = OrchardCore; User Id = userOrchard; Password = user_orc";
            var configuration = new Configuration();
            configuration.UseSqlServer(connectionString, IsolationLevel.ReadUncommitted);

            // Create a reusable store holding the configuration
            _store = new Store(configuration);
            var services = new ServiceCollection();
        }

        public void Dispose()
        {
            _store.Dispose();
        }

        [Fact]
        public void ShouldWriteAndGet()
        {

            var page = new MyPage() { Title = "Created from code", Body = "Some text", SomeDate = DateTime.Now };

            using (var session = _store.CreateSession())
            {

                session.Save(page);
            }

            using (var session = _store.CreateSession())
            {
                var p = session.Query<MyPage>().FirstOrDefaultAsync();
                p.Wait();
                Assert.Equal(page.Body, p.Result.Body);
            }

            using (var session = _store.CreateSession())
            {
                session.Delete(page);
            }
        }

        [Fact]
        public void ShouldGetByDate()
        {
            var page1 = new MyPage() { Title = "Created from code", Body = "Some text 1", SomeDate = new DateTime(1999, 5, 10) };
            var page2 = new MyPage() { Title = "Created from code", Body = "Some text 2", SomeDate = new DateTime(2010, 3, 1) };
            var page3 = new MyPage() { Title = "Created from code", Body = "Some text 3", SomeDate = new DateTime(2011, 3, 16) };

            using (var session = _store.CreateSession())
            {
                try
                {

                    session.Save(page1);
                    session.Save(page2);
                    session.Save(page3);

                    var builder = new SchemaBuilder(session);

                    builder.DropMapIndexTable(nameof(MyPageByYear));

                    builder.CreateMapIndexTable(nameof(MyPageByYear), c => c
                        .Column<string>("Title")
                        .Column<string>("Body")
                        .Column<int>("Year")
                    );

                    _store.RegisterIndexes<MyPageIndexProvider>();


                    var pages = session.Query<MyPage, MyPageByYear>().Where(p => p.Year > 1999).ListAsync().Result;
                    Assert.Equal(2, pages.Count());

                    var onePage = session.Query<MyPage, MyPageByYear>().Where(p => p.Year == 2011).FirstOrDefaultAsync().Result;
                    Assert.Equal("Some text 3", onePage.Body);

                }
                finally
                {
                    session.Delete(page1);
                    session.Delete(page2);
                    session.Delete(page3);
                }
            }
        }

    }
}
