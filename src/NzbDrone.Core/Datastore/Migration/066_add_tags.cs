using FluentMigrator;
using NzbDrone.Core.Datastore.Migration.Framework;

namespace NzbDrone.Core.Datastore.Migration
{
    [Migration(67)]
    public class add_release_restrictions : NzbDroneMigrationBase
    {
        protected override void MainDbUpgrade()
        {
            Create.TableForModel("ReleaseRestrictions")
                  .WithColumn("Label").AsString().NotNullable()
                  .WithColumn("Required").AsString().NotNullable()
                  .WithColumn("Preferred").AsString().NotNullable()
                  .WithColumn("Ignored").AsString().NotNullable()
                  .WithColumn("Tags").AsString().NotNullable();
        }
    }
}
