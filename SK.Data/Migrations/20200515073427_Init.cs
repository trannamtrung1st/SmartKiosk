using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SK.Data.Migrations
{
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(unicode: false, maxLength: 100, nullable: false),
                    Name = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(nullable: true),
                    DisplayName = table.Column<string>(nullable: true),
                    RoleType = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(unicode: false, maxLength: 100, nullable: false),
                    UserName = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(maxLength: 256, nullable: true),
                    Email = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(nullable: false),
                    PasswordHash = table.Column<string>(nullable: true),
                    SecurityStamp = table.Column<string>(nullable: true),
                    ConcurrencyStamp = table.Column<string>(nullable: true),
                    PhoneNumber = table.Column<string>(nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(nullable: false),
                    TwoFactorEnabled = table.Column<bool>(nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(nullable: true),
                    LockoutEnabled = table.Column<bool>(nullable: false),
                    AccessFailedCount = table.Column<int>(nullable: false),
                    FullName = table.Column<string>(nullable: true),
                    ActivationCode = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EntityCategory",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EntityCategory", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Location",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(maxLength: 255, nullable: true),
                    Name = table.Column<string>(maxLength: 255, nullable: true),
                    Address = table.Column<string>(maxLength: 255, nullable: true),
                    Description = table.Column<string>(nullable: true),
                    Archived = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Location", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Owner",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(maxLength: 255, nullable: true),
                    Code = table.Column<string>(unicode: false, maxLength: 100, nullable: true),
                    Description = table.Column<string>(maxLength: 500, nullable: true),
                    Phone = table.Column<string>(maxLength: 100, nullable: true),
                    Archived = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Owner", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ResourceType",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResourceType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(nullable: false),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(nullable: false),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(nullable: false),
                    ProviderKey = table.Column<string>(nullable: false),
                    ProviderDisplayName = table.Column<string>(nullable: true),
                    UserId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(nullable: false),
                    RoleId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(nullable: false),
                    LoginProvider = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EntityCategoryContent",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(maxLength: 255, nullable: true),
                    Lang = table.Column<string>(unicode: false, maxLength: 2, nullable: true),
                    CategoryId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EntityCategoryContent", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ECC_Category",
                        column: x => x.CategoryId,
                        principalTable: "EntityCategory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Building",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(maxLength: 255, nullable: true),
                    Name = table.Column<string>(maxLength: 255, nullable: true),
                    Description = table.Column<string>(nullable: true),
                    LocationId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Building", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Building__Location",
                        column: x => x.LocationId,
                        principalTable: "Location",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Config",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(unicode: false, maxLength: 100, nullable: true),
                    Name = table.Column<string>(maxLength: 255, nullable: true),
                    Description = table.Column<string>(maxLength: 500, nullable: true),
                    ScreenSaverPlaylist = table.Column<string>(nullable: true),
                    HomeConfig = table.Column<string>(nullable: true),
                    MapConfig = table.Column<string>(nullable: true),
                    ProgramEventConfig = table.Column<string>(nullable: true),
                    NotiConfig = table.Column<string>(nullable: true),
                    ContactConfig = table.Column<string>(nullable: true),
                    LocationId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Config", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Config_Location",
                        column: x => x.LocationId,
                        principalTable: "Location",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Schedule",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(maxLength: 255, nullable: true),
                    Code = table.Column<string>(unicode: false, maxLength: 100, nullable: true),
                    Description = table.Column<string>(maxLength: 500, nullable: true),
                    LocationId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Schedule", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Schedule_Location",
                        column: x => x.LocationId,
                        principalTable: "Location",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Post",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ImageUrl = table.Column<string>(nullable: true),
                    Type = table.Column<int>(unicode: false, maxLength: 100, nullable: false),
                    CreatedTime = table.Column<DateTime>(type: "datetime", nullable: false),
                    VisibleTime = table.Column<DateTime>(nullable: true),
                    Archived = table.Column<bool>(nullable: false),
                    LocationId = table.Column<int>(nullable: false),
                    OwnerId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Post", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Post_Location",
                        column: x => x.LocationId,
                        principalTable: "Location",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Post_Owner",
                        column: x => x.OwnerId,
                        principalTable: "Owner",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ResourceTypeContent",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(maxLength: 255, nullable: true),
                    Lang = table.Column<string>(unicode: false, maxLength: 2, nullable: true),
                    ResourceTypeId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResourceTypeContent", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RTC_ResourceType",
                        column: x => x.ResourceTypeId,
                        principalTable: "ResourceType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Floor",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(maxLength: 255, nullable: true),
                    Name = table.Column<string>(maxLength: 255, nullable: true),
                    Description = table.Column<string>(nullable: true),
                    LocationId = table.Column<int>(nullable: false),
                    BuildingId = table.Column<int>(nullable: false),
                    FloorPlanSvg = table.Column<string>(type: "ntext", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Floor", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Floor_Building",
                        column: x => x.BuildingId,
                        principalTable: "Building",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Floor_Location",
                        column: x => x.LocationId,
                        principalTable: "Location",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ScheduleDetail",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(maxLength: 255, nullable: true),
                    FromTime = table.Column<DateTime>(type: "datetime", nullable: true),
                    ToTime = table.Column<DateTime>(type: "datetime", nullable: true),
                    IsDefault = table.Column<bool>(nullable: true),
                    ScheduleId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScheduleDetail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ScheduleDetail_Schedule",
                        column: x => x.ScheduleId,
                        principalTable: "Schedule",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PostContent",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PostId = table.Column<int>(nullable: false),
                    Lang = table.Column<string>(unicode: false, maxLength: 2, nullable: true),
                    Title = table.Column<string>(maxLength: 255, nullable: true),
                    Content = table.Column<string>(type: "ntext", nullable: true),
                    Description = table.Column<string>(maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PostContent", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PostContent_Post",
                        column: x => x.PostId,
                        principalTable: "Post",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Area",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(maxLength: 255, nullable: true),
                    Code = table.Column<string>(unicode: false, maxLength: 100, nullable: true),
                    Description = table.Column<string>(maxLength: 500, nullable: true),
                    FloorId = table.Column<int>(nullable: false),
                    BuildingId = table.Column<int>(nullable: false),
                    LocationId = table.Column<int>(nullable: false),
                    Archived = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Area", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Area_Floor",
                        column: x => x.FloorId,
                        principalTable: "Floor",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Area_Location",
                        column: x => x.LocationId,
                        principalTable: "Location",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ScheduleWeekConfig",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FromTime = table.Column<TimeSpan>(nullable: true),
                    ToTime = table.Column<TimeSpan>(nullable: true),
                    AllDay = table.Column<bool>(nullable: true),
                    FromDayOfWeek = table.Column<int>(nullable: true),
                    ToDayOfWeek = table.Column<int>(nullable: true),
                    ScheduleDetailId = table.Column<int>(nullable: true),
                    ConfigId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScheduleWeekConfig", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SWC_Config",
                        column: x => x.ConfigId,
                        principalTable: "Config",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SWC_ScheduleDetail",
                        column: x => x.ScheduleDetailId,
                        principalTable: "ScheduleDetail",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Device",
                columns: table => new
                {
                    Id = table.Column<string>(unicode: false, maxLength: 100, nullable: false),
                    Code = table.Column<string>(unicode: false, maxLength: 100, nullable: true),
                    Name = table.Column<string>(maxLength: 255, nullable: true),
                    Description = table.Column<string>(maxLength: 500, nullable: true),
                    AreaId = table.Column<int>(nullable: true),
                    BuildingId = table.Column<int>(nullable: true),
                    FloorId = table.Column<int>(nullable: true),
                    LocationId = table.Column<int>(nullable: true),
                    Lat = table.Column<double>(nullable: true),
                    Lon = table.Column<double>(nullable: true),
                    ScheduleId = table.Column<int>(nullable: true),
                    CurrentFcmToken = table.Column<string>(unicode: false, maxLength: 255, nullable: true),
                    AccessToken = table.Column<string>(unicode: false, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Device", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Device_Area",
                        column: x => x.AreaId,
                        principalTable: "Area",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Device_Floor_FloorId",
                        column: x => x.FloorId,
                        principalTable: "Floor",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Device_User",
                        column: x => x.Id,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Device_Location",
                        column: x => x.LocationId,
                        principalTable: "Location",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Device_Schedule",
                        column: x => x.ScheduleId,
                        principalTable: "Schedule",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Resource",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(unicode: false, maxLength: 100, nullable: true),
                    Phone = table.Column<string>(maxLength: 100, nullable: true),
                    LogoUrl = table.Column<string>(unicode: false, maxLength: 1000, nullable: true),
                    ImageUrl = table.Column<string>(unicode: false, maxLength: 1000, nullable: true),
                    Archived = table.Column<bool>(nullable: false),
                    TypeId = table.Column<int>(nullable: false),
                    OwnerId = table.Column<int>(nullable: false),
                    LocationId = table.Column<int>(nullable: false),
                    AreaId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Resource", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Resource_Area",
                        column: x => x.AreaId,
                        principalTable: "Area",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Resource_Location",
                        column: x => x.LocationId,
                        principalTable: "Location",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Resource_Owner",
                        column: x => x.OwnerId,
                        principalTable: "Owner",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Resource_ResourceType",
                        column: x => x.TypeId,
                        principalTable: "ResourceType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CategoriesOfResources",
                columns: table => new
                {
                    CategoryId = table.Column<int>(nullable: false),
                    ResourceId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoriesOfResources", x => new { x.CategoryId, x.ResourceId });
                    table.ForeignKey(
                        name: "FK_COR_Category",
                        column: x => x.CategoryId,
                        principalTable: "EntityCategory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_COR_Resource",
                        column: x => x.ResourceId,
                        principalTable: "Resource",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PostResource",
                columns: table => new
                {
                    PostId = table.Column<int>(nullable: false),
                    ResourceId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PostResource", x => new { x.PostId, x.ResourceId });
                    table.ForeignKey(
                        name: "FK_PostResource_Post",
                        column: x => x.PostId,
                        principalTable: "Post",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PostResource_Resource",
                        column: x => x.ResourceId,
                        principalTable: "Resource",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ResourceContent",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(maxLength: 255, nullable: true),
                    Description = table.Column<string>(maxLength: 500, nullable: true),
                    Content = table.Column<string>(type: "ntext", nullable: true),
                    Lang = table.Column<string>(unicode: false, maxLength: 2, nullable: true),
                    ResourceId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResourceContent", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ResourceContent_Resource",
                        column: x => x.ResourceId,
                        principalTable: "Resource",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "DisplayName", "Name", "NormalizedName", "RoleType" },
                values: new object[,]
                {
                    { "00ffde62-7185-4ece-806a-f26731200061", "e577053b0fce-4d01-abdc-f96a0510e369", "Quản lí thiết bị", "DeviceManager", "DEVICEMANAGER", "User" },
                    { "0a4fdf5e-ba25-4368-94ff-0c7f7c863e38", "e8029a352c2f-423d-979b-cd43051c64af", "Quản lí dữ liệu", "DataManager", "DATAMANAGER", "User" },
                    { "283732614265-4ea2-88da-1cc848f9a373", "1d03745d-f1c2-439f-825e-0c53f725fd42", "Thiết bị", "Device", "DEVICE", "Device" },
                    { "5ebe8c26-b2ba-418a-b6aa-3829209658ed", "bace6fcf92d5-4ca5-8ac3-b25dcf824788", "Quản lí tài khoản", "UserManager", "USERMANAGER", "User" },
                    { "6166b3dc-a14c-43cf-bb55-649c5fbe3641", "086bbeee-d406-4259-96e2-9ec7fa613c15", "Quản lí tòa nhà", "BuildingManager", "BUILDINGMANAGER", "User" },
                    { "95c1365a-d715-42d7-97d6-73f6ce72d0e2", "68336549262b-4809-9bf3-7e192a3c107c", "Quản lí địa điểm", "LocationManager", "LOCATIONMANAGER", "User" },
                    { "9e45c4b2-5d23-4c9e-8a8a-2c2cdbd2cee0", "9136a9f2-1a1c-4b10-80ce-62752091dcee", "Quản lí cấu hình", "ConfigManager", "CONFIGMANAGER", "User" },
                    { "c7a0931d-7177-4b05-8468-0de8f8ed8df5", "d52b51a770c6-4e19-8c9c-1b34f1e3d0f4", "Thống kê", "ReportManager", "REPORTMANAGER", "User" },
                    { "cb63ac9f-c0d5-40b2-b40b-fc633e07cf60", "2b321caa-c4da-4c5b-a005-506153a9ef9f", "Quản lí chủ sở hữu", "OwnerManager", "OWNERMANAGER", "User" },
                    { "ddee2ac5-7074-4b29-9eab-69812970a1db", "eaab9c953a81-4e69-9e52-2117f42e59bb", "Quản lí ứng dụng", "AppManager", "APPMANAGER", "User" },
                    { "f53460d7-6741-4bbe-841a-b561ff43d33c", "fc5c8dfab84f-41c7-b8b7-f054bff0d474", "Quản lí lịch phát", "ScheduleManager", "SCHEDULEMANAGER", "User" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ActivationCode", "ConcurrencyStamp", "Email", "EmailConfirmed", "FullName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "b4895be6-a23b-4e4a-b493-f38bc4d504b3", 0, null, "2e1344110c4b-4ba6-b0fa-7b68905c2b40", null, false, null, true, null, null, "ADMIN", "AQAAAAEAACcQAAAAEAx05XG3wXdmfE+mMJXWXgh19giJfCDi0scE5xY87mx6xHonwEbIHBdBkbli6tpIew==", null, false, "D6AAM3EXCWDIHEGG3DEWNSKO7LHGCPKJ", false, "admin" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "UserId", "RoleId" },
                values: new object[,]
                {
                    { "b4895be6-a23b-4e4a-b493-f38bc4d504b3", "00ffde62-7185-4ece-806a-f26731200061" },
                    { "b4895be6-a23b-4e4a-b493-f38bc4d504b3", "0a4fdf5e-ba25-4368-94ff-0c7f7c863e38" },
                    { "b4895be6-a23b-4e4a-b493-f38bc4d504b3", "5ebe8c26-b2ba-418a-b6aa-3829209658ed" },
                    { "b4895be6-a23b-4e4a-b493-f38bc4d504b3", "6166b3dc-a14c-43cf-bb55-649c5fbe3641" },
                    { "b4895be6-a23b-4e4a-b493-f38bc4d504b3", "95c1365a-d715-42d7-97d6-73f6ce72d0e2" },
                    { "b4895be6-a23b-4e4a-b493-f38bc4d504b3", "9e45c4b2-5d23-4c9e-8a8a-2c2cdbd2cee0" },
                    { "b4895be6-a23b-4e4a-b493-f38bc4d504b3", "c7a0931d-7177-4b05-8468-0de8f8ed8df5" },
                    { "b4895be6-a23b-4e4a-b493-f38bc4d504b3", "cb63ac9f-c0d5-40b2-b40b-fc633e07cf60" },
                    { "b4895be6-a23b-4e4a-b493-f38bc4d504b3", "ddee2ac5-7074-4b29-9eab-69812970a1db" },
                    { "b4895be6-a23b-4e4a-b493-f38bc4d504b3", "f53460d7-6741-4bbe-841a-b561ff43d33c" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Area_FloorId",
                table: "Area",
                column: "FloorId");

            migrationBuilder.CreateIndex(
                name: "IX_Area_LocationId",
                table: "Area",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Building_LocationId",
                table: "Building",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_CategoriesOfResources_ResourceId",
                table: "CategoriesOfResources",
                column: "ResourceId");

            migrationBuilder.CreateIndex(
                name: "IX_Config_LocationId",
                table: "Config",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_Device_AreaId",
                table: "Device",
                column: "AreaId");

            migrationBuilder.CreateIndex(
                name: "IX_Device_FloorId",
                table: "Device",
                column: "FloorId");

            migrationBuilder.CreateIndex(
                name: "IX_Device_LocationId",
                table: "Device",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_Device_ScheduleId",
                table: "Device",
                column: "ScheduleId");

            migrationBuilder.CreateIndex(
                name: "IX_EntityCategoryContent_CategoryId",
                table: "EntityCategoryContent",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Floor_BuildingId",
                table: "Floor",
                column: "BuildingId");

            migrationBuilder.CreateIndex(
                name: "IX_Floor_LocationId",
                table: "Floor",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_Post_LocationId",
                table: "Post",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_Post_OwnerId",
                table: "Post",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_PostContent_PostId",
                table: "PostContent",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_PostResource_ResourceId",
                table: "PostResource",
                column: "ResourceId");

            migrationBuilder.CreateIndex(
                name: "IX_Resource_AreaId",
                table: "Resource",
                column: "AreaId");

            migrationBuilder.CreateIndex(
                name: "IX_Resource_LocationId",
                table: "Resource",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_Resource_OwnerId",
                table: "Resource",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_Resource_TypeId",
                table: "Resource",
                column: "TypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ResourceContent_ResourceId",
                table: "ResourceContent",
                column: "ResourceId");

            migrationBuilder.CreateIndex(
                name: "IX_ResourceTypeContent_ResourceTypeId",
                table: "ResourceTypeContent",
                column: "ResourceTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Schedule_LocationId",
                table: "Schedule",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_ScheduleDetail_ScheduleId",
                table: "ScheduleDetail",
                column: "ScheduleId");

            migrationBuilder.CreateIndex(
                name: "IX_ScheduleWeekConfig_ConfigId",
                table: "ScheduleWeekConfig",
                column: "ConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_ScheduleWeekConfig_ScheduleDetailId",
                table: "ScheduleWeekConfig",
                column: "ScheduleDetailId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "CategoriesOfResources");

            migrationBuilder.DropTable(
                name: "Device");

            migrationBuilder.DropTable(
                name: "EntityCategoryContent");

            migrationBuilder.DropTable(
                name: "PostContent");

            migrationBuilder.DropTable(
                name: "PostResource");

            migrationBuilder.DropTable(
                name: "ResourceContent");

            migrationBuilder.DropTable(
                name: "ResourceTypeContent");

            migrationBuilder.DropTable(
                name: "ScheduleWeekConfig");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "EntityCategory");

            migrationBuilder.DropTable(
                name: "Post");

            migrationBuilder.DropTable(
                name: "Resource");

            migrationBuilder.DropTable(
                name: "Config");

            migrationBuilder.DropTable(
                name: "ScheduleDetail");

            migrationBuilder.DropTable(
                name: "Area");

            migrationBuilder.DropTable(
                name: "Owner");

            migrationBuilder.DropTable(
                name: "ResourceType");

            migrationBuilder.DropTable(
                name: "Schedule");

            migrationBuilder.DropTable(
                name: "Floor");

            migrationBuilder.DropTable(
                name: "Building");

            migrationBuilder.DropTable(
                name: "Location");
        }
    }
}
