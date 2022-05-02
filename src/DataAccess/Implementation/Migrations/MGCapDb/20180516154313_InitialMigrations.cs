using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace MGCap.DataAccess.Implementation.Migrations.MGCapDb
{
    public partial class InitialMigrations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Addresses",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AddressLine1 = table.Column<string>(maxLength: 80, nullable: true),
                    AddressLine2 = table.Column<string>(maxLength: 80, nullable: true),
                    City = table.Column<string>(maxLength: 80, nullable: true),
                    CountryCode = table.Column<string>(maxLength: 3, nullable: true),
                    CreatedBy = table.Column<string>(maxLength: 80, nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    Latitude = table.Column<double>(nullable: true),
                    Longitude = table.Column<double>(nullable: true),
                    State = table.Column<string>(maxLength: 80, nullable: true),
                    UpdatedBy = table.Column<string>(maxLength: 80, nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: false),
                    ZipCode = table.Column<string>(maxLength: 32, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Addresses", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Franchises",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedBy = table.Column<string>(maxLength: 80, nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    Name = table.Column<string>(maxLength: 80, nullable: true),
                    UpdatedBy = table.Column<string>(maxLength: 80, nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Franchises", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Companies",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedBy = table.Column<string>(maxLength: 80, nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    FranchiseId = table.Column<int>(nullable: true),
                    Name = table.Column<string>(maxLength: 80, nullable: true),
                    UpdatedBy = table.Column<string>(maxLength: 80, nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Companies", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Companies_Franchises_FranchiseId",
                        column: x => x.FranchiseId,
                        principalTable: "Franchises",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Contacts",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CompanyId = table.Column<int>(nullable: false),
                    CompanyName = table.Column<string>(maxLength: 80, nullable: true),
                    CreatedBy = table.Column<string>(maxLength: 80, nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    DOB = table.Column<DateTime>(nullable: true),
                    FirstName = table.Column<string>(maxLength: 80, nullable: false),
                    Gender = table.Column<string>(maxLength: 1, nullable: true),
                    Guid = table.Column<Guid>(nullable: false),
                    LastName = table.Column<string>(maxLength: 80, nullable: false),
                    MiddleName = table.Column<string>(maxLength: 80, nullable: true),
                    Notes = table.Column<string>(nullable: true),
                    Salutation = table.Column<string>(maxLength: 80, nullable: true),
                    Title = table.Column<string>(maxLength: 80, nullable: true),
                    UpdatedBy = table.Column<string>(maxLength: 80, nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contacts", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Contacts_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CustomerGroups",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CompanyId = table.Column<int>(nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 80, nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    Guid = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(maxLength: 80, nullable: false),
                    UpdatedBy = table.Column<string>(maxLength: 80, nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerGroups", x => x.ID);
                    table.ForeignKey(
                        name: "FK_CustomerGroups_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Customers",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CRHoldFlag = table.Column<string>(nullable: true),
                    Code = table.Column<string>(maxLength: 10, nullable: false),
                    CompanyId = table.Column<int>(nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 80, nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreditLimit = table.Column<double>(nullable: false),
                    CreditTerms = table.Column<string>(nullable: true),
                    FinanceCharges = table.Column<bool>(nullable: false),
                    FixedMarkupRate = table.Column<bool>(nullable: false),
                    GracePeriodInDays = table.Column<int>(nullable: false),
                    Guid = table.Column<Guid>(nullable: false),
                    InformSalesRepMinMargin = table.Column<bool>(nullable: false),
                    InsuredUpTo = table.Column<double>(nullable: true),
                    InsurerName = table.Column<string>(nullable: true),
                    IsGenericAccount = table.Column<bool>(nullable: false),
                    MinimumProfitMargin = table.Column<double>(nullable: false),
                    Name = table.Column<string>(maxLength: 80, nullable: false),
                    Notes = table.Column<string>(nullable: true),
                    PONumberRequired = table.Column<bool>(nullable: false),
                    PricingColumn = table.Column<string>(maxLength: 1, nullable: true),
                    PricingMethod = table.Column<string>(maxLength: 1, nullable: true),
                    PricingRow = table.Column<int>(nullable: false),
                    ShowPricesOnShipper = table.Column<bool>(nullable: false),
                    StatusId = table.Column<int>(nullable: false),
                    UpdatedBy = table.Column<string>(maxLength: 80, nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customers", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Customers_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Departments",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CompanyId = table.Column<int>(nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 80, nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    Guid = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(maxLength: 80, nullable: true),
                    UpdatedBy = table.Column<string>(maxLength: 80, nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Departments", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Departments_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VendorGroups",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CompanyId = table.Column<int>(nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 80, nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    Guid = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(maxLength: 80, nullable: false),
                    UpdatedBy = table.Column<string>(maxLength: 80, nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VendorGroups", x => x.ID);
                    table.ForeignKey(
                        name: "FK_VendorGroups_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Vendors",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AccountNumber = table.Column<string>(maxLength: 128, nullable: true),
                    Code = table.Column<string>(maxLength: 32, nullable: false),
                    CompanyId = table.Column<int>(nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 80, nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreditLimit = table.Column<double>(nullable: false),
                    DefaultGLAccountNumber1 = table.Column<string>(maxLength: 10, nullable: true),
                    DefaultGLAccountNumber2 = table.Column<string>(maxLength: 10, nullable: true),
                    FEIN = table.Column<string>(maxLength: 80, nullable: true),
                    Guid = table.Column<Guid>(nullable: false),
                    Is1099 = table.Column<bool>(nullable: false),
                    IsPerson = table.Column<bool>(nullable: false),
                    IsSensitiveAccount = table.Column<bool>(nullable: false),
                    LeadTime = table.Column<int>(nullable: false),
                    Name = table.Column<string>(maxLength: 80, nullable: false),
                    Notes = table.Column<string>(nullable: true),
                    SSN = table.Column<string>(maxLength: 9, nullable: true),
                    StatusId = table.Column<int>(nullable: false),
                    TermsDaysOrProx = table.Column<string>(maxLength: 1, nullable: true),
                    TermsDiscDays = table.Column<int>(nullable: false),
                    TermsDiscPercent = table.Column<double>(nullable: false),
                    TermsNet = table.Column<int>(nullable: false),
                    UpdatedBy = table.Column<string>(maxLength: 80, nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: false),
                    VendorTypeId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vendors", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Vendors_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ContactAddresses",
                columns: table => new
                {
                    AddressId = table.Column<int>(nullable: false),
                    ContactId = table.Column<int>(nullable: false),
                    Default = table.Column<bool>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Type = table.Column<string>(maxLength: 80, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContactAddresses", x => new { x.AddressId, x.ContactId });
                    table.ForeignKey(
                        name: "FK_ContactAddresses_Addresses_AddressId",
                        column: x => x.AddressId,
                        principalTable: "Addresses",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ContactAddresses_Contacts_ContactId",
                        column: x => x.ContactId,
                        principalTable: "Contacts",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ContactEmails",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ContactId = table.Column<int>(nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 80, nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    Default = table.Column<bool>(nullable: false),
                    Email = table.Column<string>(maxLength: 128, nullable: true),
                    Type = table.Column<string>(maxLength: 80, nullable: true),
                    UpdatedBy = table.Column<string>(maxLength: 80, nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContactEmails", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ContactEmails_Contacts_ContactId",
                        column: x => x.ContactId,
                        principalTable: "Contacts",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ContactPhones",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ContactId = table.Column<int>(nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 80, nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    Default = table.Column<bool>(nullable: false),
                    Ext = table.Column<string>(maxLength: 13, nullable: true),
                    Phone = table.Column<string>(maxLength: 13, nullable: true),
                    Type = table.Column<string>(maxLength: 80, nullable: true),
                    UpdatedBy = table.Column<string>(maxLength: 80, nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContactPhones", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ContactPhones_Contacts_ContactId",
                        column: x => x.ContactId,
                        principalTable: "Contacts",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CustomerAddresses",
                columns: table => new
                {
                    AddressId = table.Column<int>(nullable: false),
                    CustomerId = table.Column<int>(nullable: false),
                    Default = table.Column<bool>(nullable: false),
                    Name = table.Column<string>(maxLength: 80, nullable: true),
                    Type = table.Column<string>(maxLength: 80, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerAddresses", x => new { x.AddressId, x.CustomerId });
                    table.ForeignKey(
                        name: "FK_CustomerAddresses_Addresses_AddressId",
                        column: x => x.AddressId,
                        principalTable: "Addresses",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CustomerAddresses_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CustomerCertificates",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedBy = table.Column<string>(maxLength: 80, nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CustomerId = table.Column<int>(nullable: false),
                    ExpirationDate = table.Column<DateTime>(nullable: false),
                    Number = table.Column<string>(maxLength: 20, nullable: false),
                    UpdatedBy = table.Column<string>(maxLength: 80, nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerCertificates", x => x.ID);
                    table.ForeignKey(
                        name: "FK_CustomerCertificates_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CustomerContacts",
                columns: table => new
                {
                    ContactId = table.Column<int>(nullable: false),
                    CustomerId = table.Column<int>(nullable: false),
                    Default = table.Column<bool>(nullable: false),
                    SelectedForMarketing = table.Column<bool>(nullable: false),
                    Type = table.Column<string>(maxLength: 80, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerContacts", x => new { x.ContactId, x.CustomerId });
                    table.ForeignKey(
                        name: "FK_CustomerContacts_Contacts_ContactId",
                        column: x => x.ContactId,
                        principalTable: "Contacts",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CustomerContacts_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "CustomerCustomerGroups",
                columns: table => new
                {
                    CustomerId = table.Column<int>(nullable: false),
                    CustomerGroupId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerCustomerGroups", x => new { x.CustomerId, x.CustomerGroupId });
                    table.UniqueConstraint("AK_CustomerCustomerGroups_CustomerGroupId_CustomerId", x => new { x.CustomerGroupId, x.CustomerId });
                    table.ForeignKey(
                        name: "FK_CustomerCustomerGroups_CustomerGroups_CustomerGroupId",
                        column: x => x.CustomerGroupId,
                        principalTable: "CustomerGroups",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CustomerCustomerGroups_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "CustomerPhones",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedBy = table.Column<string>(maxLength: 80, nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CustomerId = table.Column<int>(nullable: false),
                    Default = table.Column<bool>(nullable: false),
                    Ext = table.Column<string>(maxLength: 13, nullable: true),
                    Phone = table.Column<string>(maxLength: 13, nullable: true),
                    Type = table.Column<string>(maxLength: 80, nullable: true),
                    UpdatedBy = table.Column<string>(maxLength: 80, nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerPhones", x => x.ID);
                    table.ForeignKey(
                        name: "FK_CustomerPhones_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Employees",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Code = table.Column<string>(maxLength: 10, nullable: true),
                    CompanyId = table.Column<int>(nullable: false),
                    ContactId = table.Column<int>(nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 80, nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    DepartmentId = table.Column<int>(nullable: true),
                    Email = table.Column<string>(maxLength: 128, nullable: false),
                    EmployeeStatusId = table.Column<int>(nullable: false),
                    Guid = table.Column<Guid>(nullable: false),
                    UpdatedBy = table.Column<string>(maxLength: 80, nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employees", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Employees_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Employees_Contacts_ContactId",
                        column: x => x.ContactId,
                        principalTable: "Contacts",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_Employees_Departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Departments",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "VendorAddresses",
                columns: table => new
                {
                    AddressId = table.Column<int>(nullable: false),
                    VendorId = table.Column<int>(nullable: false),
                    Default = table.Column<bool>(nullable: false),
                    Name = table.Column<string>(maxLength: 80, nullable: true),
                    Type = table.Column<string>(maxLength: 80, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VendorAddresses", x => new { x.AddressId, x.VendorId });
                    table.ForeignKey(
                        name: "FK_VendorAddresses_Addresses_AddressId",
                        column: x => x.AddressId,
                        principalTable: "Addresses",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VendorAddresses_Vendors_VendorId",
                        column: x => x.VendorId,
                        principalTable: "Vendors",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VendorContacts",
                columns: table => new
                {
                    ContactId = table.Column<int>(nullable: false),
                    VendorId = table.Column<int>(nullable: false),
                    Default = table.Column<bool>(nullable: false),
                    Type = table.Column<string>(maxLength: 80, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VendorContacts", x => new { x.ContactId, x.VendorId });
                    table.ForeignKey(
                        name: "FK_VendorContacts_Contacts_ContactId",
                        column: x => x.ContactId,
                        principalTable: "Contacts",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VendorContacts_Vendors_VendorId",
                        column: x => x.VendorId,
                        principalTable: "Vendors",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "VendorEmails",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedBy = table.Column<string>(maxLength: 80, nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    Default = table.Column<bool>(nullable: false),
                    Email = table.Column<string>(maxLength: 128, nullable: true),
                    Type = table.Column<string>(maxLength: 80, nullable: true),
                    UpdatedBy = table.Column<string>(maxLength: 80, nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: false),
                    VendorId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VendorEmails", x => x.ID);
                    table.ForeignKey(
                        name: "FK_VendorEmails_Vendors_VendorId",
                        column: x => x.VendorId,
                        principalTable: "Vendors",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VendorPhones",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedBy = table.Column<string>(maxLength: 80, nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    Default = table.Column<bool>(nullable: false),
                    Ext = table.Column<string>(maxLength: 13, nullable: true),
                    Phone = table.Column<string>(maxLength: 13, nullable: true),
                    Type = table.Column<string>(maxLength: 80, nullable: true),
                    UpdatedBy = table.Column<string>(maxLength: 80, nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: false),
                    VendorId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VendorPhones", x => x.ID);
                    table.ForeignKey(
                        name: "FK_VendorPhones_Vendors_VendorId",
                        column: x => x.VendorId,
                        principalTable: "Vendors",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VendorVendorGroups",
                columns: table => new
                {
                    VendorId = table.Column<int>(nullable: false),
                    VendorGroupId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VendorVendorGroups", x => new { x.VendorId, x.VendorGroupId });
                    table.UniqueConstraint("AK_VendorVendorGroups_VendorGroupId_VendorId", x => new { x.VendorGroupId, x.VendorId });
                    table.ForeignKey(
                        name: "FK_VendorVendorGroups_VendorGroups_VendorGroupId",
                        column: x => x.VendorGroupId,
                        principalTable: "VendorGroups",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VendorVendorGroups_Vendors_VendorId",
                        column: x => x.VendorId,
                        principalTable: "Vendors",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "CustomerEmployees",
                columns: table => new
                {
                    EmployeeId = table.Column<int>(nullable: false),
                    CustomerId = table.Column<int>(nullable: false),
                    Type = table.Column<string>(maxLength: 128, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerEmployees", x => new { x.EmployeeId, x.CustomerId });
                    table.UniqueConstraint("AK_CustomerEmployees_CustomerId_EmployeeId", x => new { x.CustomerId, x.EmployeeId });
                    table.ForeignKey(
                        name: "FK_CustomerEmployees_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CustomerEmployees_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Companies_FranchiseId",
                table: "Companies",
                column: "FranchiseId");

            migrationBuilder.CreateIndex(
                name: "IX_ContactAddresses_ContactId",
                table: "ContactAddresses",
                column: "ContactId");

            migrationBuilder.CreateIndex(
                name: "IX_ContactEmails_ContactId",
                table: "ContactEmails",
                column: "ContactId");

            migrationBuilder.CreateIndex(
                name: "IX_ContactPhones_ContactId",
                table: "ContactPhones",
                column: "ContactId");

            migrationBuilder.CreateIndex(
                name: "IX_Contacts_CompanyId",
                table: "Contacts",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerAddresses_CustomerId",
                table: "CustomerAddresses",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerCertificates_CustomerId",
                table: "CustomerCertificates",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerContacts_CustomerId",
                table: "CustomerContacts",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerGroups_CompanyId",
                table: "CustomerGroups",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerPhones_CustomerId",
                table: "CustomerPhones",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Customers_CompanyId",
                table: "Customers",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Departments_CompanyId",
                table: "Departments",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_CompanyId",
                table: "Employees",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_ContactId",
                table: "Employees",
                column: "ContactId");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_DepartmentId",
                table: "Employees",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_VendorAddresses_VendorId",
                table: "VendorAddresses",
                column: "VendorId");

            migrationBuilder.CreateIndex(
                name: "IX_VendorContacts_VendorId",
                table: "VendorContacts",
                column: "VendorId");

            migrationBuilder.CreateIndex(
                name: "IX_VendorEmails_VendorId",
                table: "VendorEmails",
                column: "VendorId");

            migrationBuilder.CreateIndex(
                name: "IX_VendorGroups_CompanyId",
                table: "VendorGroups",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_VendorPhones_VendorId",
                table: "VendorPhones",
                column: "VendorId");

            migrationBuilder.CreateIndex(
                name: "IX_Vendors_CompanyId",
                table: "Vendors",
                column: "CompanyId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ContactAddresses");

            migrationBuilder.DropTable(
                name: "ContactEmails");

            migrationBuilder.DropTable(
                name: "ContactPhones");

            migrationBuilder.DropTable(
                name: "CustomerAddresses");

            migrationBuilder.DropTable(
                name: "CustomerCertificates");

            migrationBuilder.DropTable(
                name: "CustomerContacts");

            migrationBuilder.DropTable(
                name: "CustomerCustomerGroups");

            migrationBuilder.DropTable(
                name: "CustomerEmployees");

            migrationBuilder.DropTable(
                name: "CustomerPhones");

            migrationBuilder.DropTable(
                name: "VendorAddresses");

            migrationBuilder.DropTable(
                name: "VendorContacts");

            migrationBuilder.DropTable(
                name: "VendorEmails");

            migrationBuilder.DropTable(
                name: "VendorPhones");

            migrationBuilder.DropTable(
                name: "VendorVendorGroups");

            migrationBuilder.DropTable(
                name: "CustomerGroups");

            migrationBuilder.DropTable(
                name: "Employees");

            migrationBuilder.DropTable(
                name: "Customers");

            migrationBuilder.DropTable(
                name: "Addresses");

            migrationBuilder.DropTable(
                name: "VendorGroups");

            migrationBuilder.DropTable(
                name: "Vendors");

            migrationBuilder.DropTable(
                name: "Contacts");

            migrationBuilder.DropTable(
                name: "Departments");

            migrationBuilder.DropTable(
                name: "Companies");

            migrationBuilder.DropTable(
                name: "Franchises");
        }
    }
}
