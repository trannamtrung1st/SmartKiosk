using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Text;

namespace SK.Data.Models
{
    public partial class DataContext : IdentityDbContext<AppUser, AppRole, string,
        IdentityUserClaim<string>, AppUserRole, IdentityUserLogin<string>,
        IdentityRoleClaim<string>, IdentityUserToken<string>>
    {
        public DataContext()
        {
        }

        public DataContext(DbContextOptions<DataContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Area> Area { get; set; }
        public virtual DbSet<Building> Building { get; set; }
        public virtual DbSet<CategoriesOfResources> CategoriesOfResources { get; set; }
        public virtual DbSet<Config> Config { get; set; }
        public virtual DbSet<Device> Device { get; set; }
        public virtual DbSet<Floor> Floor { get; set; }
        public virtual DbSet<Location> Location { get; set; }
        public virtual DbSet<Owner> Owner { get; set; }
        public virtual DbSet<PostResource> PostResource { get; set; }
        public virtual DbSet<Post> Post { get; set; }
        public virtual DbSet<PostContent> PostContent { get; set; }
        public virtual DbSet<EntityCategory> EntityCategory { get; set; }
        public virtual DbSet<EntityCategoryContent> EntityCategoryContent { get; set; }
        public virtual DbSet<ResourceType> ResourceType { get; set; }
        public virtual DbSet<ResourceTypeContent> ResourceTypeContent { get; set; }
        public virtual DbSet<Resource> Resource { get; set; }
        public virtual DbSet<ResourceContent> ResourceContent { get; set; }
        public virtual DbSet<ScheduleDetail> ScheduleDetail { get; set; }
        public virtual DbSet<ScheduleWeekConfig> ScheduleWeekConfig { get; set; }
        public virtual DbSet<Schedule> Schedule { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer(DataConsts.CONN_STR)
                    .UseLazyLoadingProxies();
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<AppUser>(b =>
            {
                b.Property(e => e.Id)
                    .IsUnicode(false)
                    .HasMaxLength(100);

                // Each User can have many entries in the UserRole join table
                b.HasMany(e => e.UserRoles)
                    .WithOne(e => e.User)
                    .HasForeignKey(ur => ur.UserId)
                    .IsRequired();

                b.HasOne(e => e.LinkedDevice)
                    .WithOne(d => d.DeviceAccount)
                    .HasForeignKey<AppUser>(e => e.Id)
                    .OnDelete(DeleteBehavior.NoAction)
                    .HasConstraintName("FK_User_Device");

                b.HasData(new AppUser { Id = "b4895be6-a23b-4e4a-b493-f38bc4d504b3", UserName = "admin", NormalizedUserName = "ADMIN", EmailConfirmed = false, PasswordHash = "AQAAAAEAACcQAAAAEAx05XG3wXdmfE+mMJXWXgh19giJfCDi0scE5xY87mx6xHonwEbIHBdBkbli6tpIew==", SecurityStamp = "D6AAM3EXCWDIHEGG3DEWNSKO7LHGCPKJ", ConcurrencyStamp = "2e1344110c4b-4ba6-b0fa-7b68905c2b40", PhoneNumberConfirmed = false, TwoFactorEnabled = false, LockoutEnabled = true, AccessFailedCount = 0, ActivationCode = null });
            });

            modelBuilder.Entity<AppRole>(b =>
            {
                b.Property(e => e.Id)
                    .IsUnicode(false)
                    .HasMaxLength(100);

                // Each Role can have many entries in the UserRole join table
                b.HasMany(e => e.UserRoles)
                    .WithOne(e => e.Role)
                    .HasForeignKey(ur => ur.RoleId)
                    .IsRequired();

                b.Property(e => e.RoleType)
                    .HasConversion(v => v.ToString(), v => (RoleType)Enum.Parse(typeof(RoleType), v));

                b.HasData(new AppRole { Id = "00ffde62-7185-4ece-806a-f26731200061", Name = "DeviceManager", NormalizedName = "DEVICEMANAGER", ConcurrencyStamp = "e577053b0fce-4d01-abdc-f96a0510e369", DisplayName = "Quản lí thiết bị", RoleType = RoleType.User }
                , new AppRole { Id = "0a4fdf5e-ba25-4368-94ff-0c7f7c863e38", Name = "DataManager", NormalizedName = "DATAMANAGER", ConcurrencyStamp = "e8029a352c2f-423d-979b-cd43051c64af", DisplayName = "Quản lí dữ liệu", RoleType = RoleType.User }
                , new AppRole { Id = "283732614265-4ea2-88da-1cc848f9a373", Name = "Device", NormalizedName = "DEVICE", ConcurrencyStamp = "1d03745d-f1c2-439f-825e-0c53f725fd42", DisplayName = "Thiết bị", RoleType = RoleType.Device }
                , new AppRole { Id = "5ebe8c26-b2ba-418a-b6aa-3829209658ed", Name = "UserManager", NormalizedName = "USERMANAGER", ConcurrencyStamp = "bace6fcf92d5-4ca5-8ac3-b25dcf824788", DisplayName = "Quản lí tài khoản", RoleType = RoleType.User }
                , new AppRole { Id = "6166b3dc-a14c-43cf-bb55-649c5fbe3641", Name = "BuildingManager", NormalizedName = "BUILDINGMANAGER", ConcurrencyStamp = "086bbeee-d406-4259-96e2-9ec7fa613c15", DisplayName = "Quản lí tòa nhà", RoleType = RoleType.User }
                , new AppRole { Id = "95c1365a-d715-42d7-97d6-73f6ce72d0e2", Name = "LocationManager", NormalizedName = "LOCATIONMANAGER", ConcurrencyStamp = "68336549262b-4809-9bf3-7e192a3c107c", DisplayName = "Quản lí địa điểm", RoleType = RoleType.User }
                , new AppRole { Id = "9e45c4b2-5d23-4c9e-8a8a-2c2cdbd2cee0", Name = "ConfigManager", NormalizedName = "CONFIGMANAGER", ConcurrencyStamp = "9136a9f2-1a1c-4b10-80ce-62752091dcee", DisplayName = "Quản lí cấu hình", RoleType = RoleType.User }
                , new AppRole { Id = "c7a0931d-7177-4b05-8468-0de8f8ed8df5", Name = "ReportManager", NormalizedName = "REPORTMANAGER", ConcurrencyStamp = "d52b51a770c6-4e19-8c9c-1b34f1e3d0f4", DisplayName = "Thống kê", RoleType = RoleType.User }
                , new AppRole { Id = "cb63ac9f-c0d5-40b2-b40b-fc633e07cf60", Name = "OwnerManager", NormalizedName = "OWNERMANAGER", ConcurrencyStamp = "2b321caa-c4da-4c5b-a005-506153a9ef9f", DisplayName = "Quản lí chủ sở hữu", RoleType = RoleType.User }
                , new AppRole { Id = "ddee2ac5-7074-4b29-9eab-69812970a1db", Name = "AppManager", NormalizedName = "APPMANAGER", ConcurrencyStamp = "eaab9c953a81-4e69-9e52-2117f42e59bb", DisplayName = "Quản lí ứng dụng", RoleType = RoleType.User }
                , new AppRole { Id = "f53460d7-6741-4bbe-841a-b561ff43d33c", Name = "ScheduleManager", NormalizedName = "SCHEDULEMANAGER", ConcurrencyStamp = "fc5c8dfab84f-41c7-b8b7-f054bff0d474", DisplayName = "Quản lí lịch phát", RoleType = RoleType.User });
            });

            modelBuilder.Entity<AppUserRole>()
                .HasData(new AppUserRole() { UserId = "b4895be6-a23b-4e4a-b493-f38bc4d504b3", RoleId = "00ffde62-7185-4ece-806a-f26731200061" }
                    , new AppUserRole() { UserId = "b4895be6-a23b-4e4a-b493-f38bc4d504b3", RoleId = "0a4fdf5e-ba25-4368-94ff-0c7f7c863e38" }
                    , new AppUserRole() { UserId = "b4895be6-a23b-4e4a-b493-f38bc4d504b3", RoleId = "5ebe8c26-b2ba-418a-b6aa-3829209658ed" }
                    , new AppUserRole() { UserId = "b4895be6-a23b-4e4a-b493-f38bc4d504b3", RoleId = "6166b3dc-a14c-43cf-bb55-649c5fbe3641" }
                    , new AppUserRole() { UserId = "b4895be6-a23b-4e4a-b493-f38bc4d504b3", RoleId = "95c1365a-d715-42d7-97d6-73f6ce72d0e2" }
                    , new AppUserRole() { UserId = "b4895be6-a23b-4e4a-b493-f38bc4d504b3", RoleId = "9e45c4b2-5d23-4c9e-8a8a-2c2cdbd2cee0" }
                    , new AppUserRole() { UserId = "b4895be6-a23b-4e4a-b493-f38bc4d504b3", RoleId = "c7a0931d-7177-4b05-8468-0de8f8ed8df5" }
                    , new AppUserRole() { UserId = "b4895be6-a23b-4e4a-b493-f38bc4d504b3", RoleId = "cb63ac9f-c0d5-40b2-b40b-fc633e07cf60" }
                    , new AppUserRole() { UserId = "b4895be6-a23b-4e4a-b493-f38bc4d504b3", RoleId = "ddee2ac5-7074-4b29-9eab-69812970a1db" }
                    , new AppUserRole() { UserId = "b4895be6-a23b-4e4a-b493-f38bc4d504b3", RoleId = "f53460d7-6741-4bbe-841a-b561ff43d33c" });

            modelBuilder.Entity<Area>(entity =>
            {
                entity.Property(e => e.Code)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Name).HasMaxLength(255);

                entity.Property(e => e.Description).HasMaxLength(500);

                entity.HasOne(e => e.Floor)
                    .WithMany(a => a.Areas)
                    .HasForeignKey(e => e.FloorId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_Area_Floor");

                entity.HasOne(e => e.Location)
                    .WithMany(l => l.Areas)
                    .HasForeignKey(e => e.LocationId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_Area_Location");
            });

            modelBuilder.Entity<Building>(entity =>
            {
                entity.Property(e => e.Code).HasMaxLength(255);

                entity.Property(e => e.Name).HasMaxLength(255);

                entity.HasOne(e => e.Location)
                    .WithMany(b => b.Buildings)
                    .HasForeignKey(e => e.LocationId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_Building__Location");
            });

            modelBuilder.Entity<CategoriesOfResources>(entity =>
            {
                entity.HasKey(e => new { e.CategoryId, e.ResourceId })
                    .HasName("PK_CategoriesOfResources");

                entity.HasOne(e => e.Category)
                    .WithMany(c => c.CategoriesOfResources)
                    .HasForeignKey(e => e.CategoryId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_COR_Category");

                entity.HasOne(e => e.Resource)
                    .WithMany(c => c.CategoriesOfResources)
                    .HasForeignKey(e => e.ResourceId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_COR_Resource");
            });

            modelBuilder.Entity<Config>(entity =>
            {
                entity.Property(e => e.Code)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Description).HasMaxLength(500);

                entity.Property(e => e.Name).HasMaxLength(255);

                entity.HasOne(e => e.Location)
                    .WithMany(c => c.Configs)
                    .HasForeignKey(d => d.LocationId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_Config_Location");
            });

            modelBuilder.Entity<Device>(entity =>
            {
                entity.Property(e => e.Id)
                    .IsUnicode(false)
                    .HasMaxLength(100);

                entity.Property(e => e.AccessToken).IsUnicode(false);

                entity.Property(e => e.Code)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.CurrentFcmToken)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.Description).HasMaxLength(500);

                entity.Property(e => e.Name).HasMaxLength(255);

                entity.HasOne(e => e.Area)
                    .WithMany(d => d.Devices)
                    .HasForeignKey(e => e.AreaId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_Device_Area");

                entity.HasOne(e => e.DeviceAccount)
                    .WithOne(u => u.LinkedDevice)
                    .HasForeignKey<Device>(e => e.Id)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_Device_User");

                entity.HasOne(e => e.Location)
                    .WithMany(l => l.Devices)
                    .HasForeignKey(e => e.LocationId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_Device_Location");

                entity.HasOne(e => e.Schedule)
                    .WithMany(s => s.Devices)
                    .HasForeignKey(e => e.ScheduleId)
                    .HasConstraintName("FK_Device_Schedule");

            });

            modelBuilder.Entity<Floor>(entity =>
            {
                entity.Property(e => e.Code).HasMaxLength(255);

                entity.Property(e => e.FloorPlanSvg)
                    .IsUnicode(true)
                    .HasColumnType("ntext");

                entity.Property(e => e.Name).HasMaxLength(255);

                entity.HasOne(e => e.Building)
                    .WithMany(b => b.Floors)
                    .HasForeignKey(e => e.BuildingId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_Floor_Building");

                entity.HasOne(e => e.Location)
                    .WithMany(l => l.Floors)
                    .HasForeignKey(e => e.LocationId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_Floor_Location");
            });

            modelBuilder.Entity<Location>(entity =>
            {
                entity.Property(e => e.Address).HasMaxLength(255);

                entity.Property(e => e.Code).HasMaxLength(255);

                entity.Property(e => e.Name).HasMaxLength(255);
            });

            modelBuilder.Entity<Owner>(entity =>
            {
                entity.Property(e => e.Code)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Description).HasMaxLength(500);

                entity.Property(e => e.Name).HasMaxLength(255);

                entity.Property(e => e.Phone).HasMaxLength(100);
            });

            modelBuilder.Entity<PostResource>(entity =>
            {
                entity.HasKey(e => new { e.PostId, e.ResourceId })
                    .HasName("PK_PostResource");

                entity.HasOne(e => e.Post)
                    .WithMany(p => p.PostResources)
                    .HasForeignKey(e => e.PostId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_PostResource_Post");

                entity.HasOne(d => d.Resource)
                    .WithMany(p => p.PostResources)
                    .HasForeignKey(d => d.ResourceId)
                    .HasConstraintName("FK_PostResource_Resource")
                    .HasConstraintName("FK_PostResource_Resource");
            });

            modelBuilder.Entity<Post>(entity =>
            {
                entity.Property(e => e.CreatedTime).HasColumnType("datetime");

                entity.Property(e => e.Type)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.HasOne(d => d.Location)
                    .WithMany(p => p.Posts)
                    .HasForeignKey(d => d.LocationId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_Post_Location");

                entity.HasOne(d => d.Owner)
                    .WithMany(p => p.Posts)
                    .HasForeignKey(d => d.OwnerId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_Post_Owner");
            });

            modelBuilder.Entity<PostContent>(entity =>
            {
                entity.Property(e => e.Lang).HasMaxLength(2).IsUnicode(false);
                entity.Property(e => e.Description).HasMaxLength(500);
                entity.Property(e => e.Title).HasMaxLength(255);
                entity.Property(e => e.Content).HasColumnType("ntext");
                entity.HasOne(p => p.Post)
                        .WithMany(p => p.Contents)
                        .HasForeignKey(p => p.PostId)
                        .OnDelete(DeleteBehavior.Restrict)
                        .HasConstraintName("FK_PostContent_Post");
            });

            modelBuilder.Entity<EntityCategory>(entity => { });

            modelBuilder.Entity<EntityCategoryContent>(entity =>
            {
                entity.Property(e => e.Name).HasMaxLength(255);
                entity.Property(e => e.Lang).HasMaxLength(2).IsUnicode(false);
                entity.HasOne(p => p.EntityCategory)
                        .WithMany(p => p.Contents)
                        .HasForeignKey(p => p.CategoryId)
                        .OnDelete(DeleteBehavior.Restrict)
                        .HasConstraintName("FK_ECC_Category");
            });

            modelBuilder.Entity<ResourceType>(entity => { });

            modelBuilder.Entity<ResourceTypeContent>(entity =>
            {
                entity.Property(e => e.Name).HasMaxLength(255);
                entity.Property(e => e.Lang).HasMaxLength(2).IsUnicode(false);
                entity.HasOne(p => p.ResourceType)
                        .WithMany(p => p.Contents)
                        .HasForeignKey(p => p.ResourceTypeId)
                        .OnDelete(DeleteBehavior.Restrict)
                        .HasConstraintName("FK_RTC_ResourceType");
            });

            modelBuilder.Entity<Resource>(entity =>
            {
                entity.Property(e => e.Code)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.ImageUrl)
                    .HasMaxLength(1000)
                    .IsUnicode(false);

                entity.Property(e => e.LogoUrl)
                    .HasMaxLength(1000)
                    .IsUnicode(false);

                entity.Property(e => e.Phone).HasMaxLength(100);

                entity.HasOne(e => e.Area)
                    .WithMany(a => a.Resources)
                    .HasForeignKey(e => e.AreaId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_Resource_Area");

                entity.HasOne(e => e.Location)
                    .WithMany(l => l.Resources)
                    .HasForeignKey(e => e.LocationId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_Resource_Location");

                entity.HasOne(e => e.Owner)
                    .WithMany(o => o.Resources)
                    .HasForeignKey(e => e.OwnerId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_Resource_Owner");

                entity.HasOne(d => d.ResourceType)
                    .WithMany(p => p.Resources)
                    .HasForeignKey(d => d.TypeId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_Resource_ResourceType");
            });

            modelBuilder.Entity<ResourceContent>(entity =>
            {
                entity.Property(e => e.Description).HasMaxLength(500);
                entity.Property(e => e.Name).HasMaxLength(255);
                entity.Property(e => e.Content).HasColumnType("ntext");
                entity.Property(e => e.Lang).HasMaxLength(2).IsUnicode(false);
                entity.HasOne(p => p.Resource)
                        .WithMany(p => p.Contents)
                        .HasForeignKey(p => p.ResourceId)
                        .OnDelete(DeleteBehavior.Restrict)
                        .HasConstraintName("FK_ResourceContent_Resource");
            });

            modelBuilder.Entity<ScheduleDetail>(entity =>
            {
                entity.Property(e => e.FromTime).HasColumnType("datetime");

                entity.Property(e => e.Name).HasMaxLength(255);

                entity.Property(e => e.ToTime).HasColumnType("datetime");

                entity.HasOne(d => d.Schedule)
                    .WithMany(p => p.ScheduleDetails)
                    .HasForeignKey(d => d.ScheduleId)
                    .HasConstraintName("FK_ScheduleDetail_Schedule");
            });

            modelBuilder.Entity<ScheduleWeekConfig>(entity =>
            {
                entity.HasOne(d => d.Config)
                    .WithMany(p => p.ScheduleWeekConfigs)
                    .HasForeignKey(d => d.ConfigId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_SWC_Config");

                entity.HasOne(d => d.ScheduleDetail)
                    .WithMany(p => p.ScheduleWeekConfigs)
                    .HasForeignKey(d => d.ScheduleDetailId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_SWC_ScheduleDetail");
            });

            modelBuilder.Entity<Schedule>(entity =>
            {
                entity.Property(e => e.Code)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Description).HasMaxLength(500);

                entity.Property(e => e.Name).HasMaxLength(255);

                entity.HasOne(d => d.Location)
                    .WithMany(p => p.Schedules)
                    .HasForeignKey(d => d.LocationId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_Schedule_Location");
            });
        }
    }

    public class DbContextFactory : IDesignTimeDbContextFactory<DataContext>
    {

        public DataContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<DataContext>();
            optionsBuilder.UseSqlServer(DataConsts.CONN_STR);
            return new DataContext(optionsBuilder.Options);
        }
    }
}
