using Microsoft.EntityFrameworkCore.Migrations;

namespace MGCap.DataAccess.Implementation.Migrations.MGCapDb
{
    public partial class AddCompanyEmailSignature : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EmailSignature",
                table: "CompanySettings",
                defaultValue: @"
<div style='font-family:&quot;Calibri Light&quot;,&quot;Helvetica Light&quot;,sans-serif;font-size:12pt;color:rgb(0,0,0)'>
   <div class='adM'></div>
   <p style='line-height:1.44;color:rgb(80,0,80)'><span style='font-family:Arial;font-size:13.3333px;color:rgb(0,0,0)'>Customer Service Specialist
      </span>
   </p>
   <p dir='ltr' style='line-height:1.656;color:rgb(80,0,80)'><img height='55px' style='width:266.129px' src='https://lh4.googleusercontent.com/oGKxSFC90avS94UFDBlZxg95Ee-imfbbs0rcDjUN4BUnN7BNPiqP8dBvwRE1D4jtCojdIVy4l_hnfysIvHRLCOt-gHQ_XOHbmEQvW0SUgD967R5DWSBhQcdBijj_mN1HvgeilcLo' class='CToWUd'><br></p>
   <p style='line-height:1.44;color:rgb(80,0,80)'><span style='font-size:11pt;font-family:Arial;font-weight:700;color:rgb(0,133,61)'>For service requests click
      </span><a href='https://mgcap-wo-form.azurewebsites.net/' rel='noreferrer' style='color:rgb(17,85,204)' target='_blank' data-saferedirecturl='https://www.google.com/url?q=https://mgcap-wo-form.azurewebsites.net/&amp;source=gmail&amp;ust=1647985643966000&amp;usg=AOvVaw2mpHPublGW-rCYVnE-hTCq'><span style='font-size:11pt;font-family:Arial;font-weight:700;color:rgb(0,133,61)'>HERE</span></a>
   </p>
   <p style='line-height:1.44;color:rgb(80,0,80)'><span style='font-size:11pt;font-family:Calibri,sans-serif;color:rgb(0,0,0)'><a href='https://www.google.com/maps/search/110+Pheasant+Wood+Ct+Ste+D+Morrisville,+NC+27560?entry=gmail&amp;source=g' style='color:rgb(17,85,204)' target='_blank' data-saferedirecturl='https://www.google.com/url?q=https://www.google.com/maps/search/110%2BPheasant%2BWood%2BCt%2BSte%2BD%2BMorrisville,%2BNC%2B27560?entry%3Dgmail%26source%3Dg&amp;source=gmail&amp;ust=1647985643966000&amp;usg=AOvVaw1i3fnxME89973F_WnFrUdK'>110
      Pheasant Wood Ct Ste D Morrisville, NC 27560</a></span>
   </p>
   <p style='line-height:1.44;color:rgb(80,0,80)'><span style='font-size:11pt;font-family:Calibri,sans-serif;color:rgb(0,0,0)'>C: 919 618-6879</span><span style='font-size:11pt;font-family:Calibri,sans-serif;font-weight:700;color:rgb(0,133,61)'>|</span><span style='font-size:11pt;font-family:Calibri,sans-serif;color:rgb(0,0,0)'>&nbsp;O:
      919 461-8573 </span><span style='font-size:11pt;font-family:Calibri,sans-serif;font-weight:700;color:rgb(0,133,61)'>|</span><span style='font-size:11pt;font-family:Calibri,sans-serif;color:rgb(0,0,0)'>&nbsp;F: 919 467-0837</span>
   </p>
   <p style='line-height:1.44;color:rgb(80,0,80)'><span style='font-size:11pt;font-family:Calibri,sans-serif;color:rgb(0,0,0)'><a href='mailto:sugeidi@mgcapitalmain.com' rel='noreferrer' style='color:rgb(17,85,204)' target='_blank'>requests@mgcapitalmain.com</a>&nbsp;</span><span style='font-size:11pt;font-family:Calibri,sans-serif;font-weight:700;color:rgb(0,133,61)'>|</span><span style='font-size:11pt;font-family:Calibri,sans-serif;color:rgb(0,0,0)'>&nbsp;</span><span style='color:rgb(0,0,0);font-size:11pt;font-family:Calibri,sans-serif'><a href='http://www.mgcapitalmain.com/' rel='noreferrer' style='color:rgb(17,85,204)' target='_blank' data-saferedirecturl='https://www.google.com/url?q=http://www.mgcapitalmain.com/&amp;source=gmail&amp;ust=1647985643966000&amp;usg=AOvVaw0JXm7KFTMMsOGfkY-Wv0mO'>w<wbr>ww.mgcapitalmain.com</a></span></p>
   <span></span><br>
</div>
                ",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EmailSignature",
                table: "CompanySettings");
        }
    }
}
