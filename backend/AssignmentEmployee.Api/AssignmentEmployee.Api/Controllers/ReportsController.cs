using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;
using AssignmentEmployee.Api.Data;

namespace AssignmentEmployee.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ReportsController : ControllerBase
    {
        private readonly ApplicationDbContext _db;

        public ReportsController(ApplicationDbContext db)
        {
            _db = db;
        }

        // GET: api/reports/employees/csv
        [HttpGet("employees/csv")]
        public async Task<IActionResult> DownloadEmployeesCsv()
        {
            var idClaim = User.FindFirst("id")?.Value;
            if (!int.TryParse(idClaim, out var userId))
                return Unauthorized();

            var employees = await _db.Employees
                .Include(e => e.Department)
                .Where(e => e.UserId == userId)
                .ToListAsync();

            var sb = new StringBuilder();
            sb.AppendLine("Id,FullName,Email,Salary,Department");

            foreach (var e in employees)
            {
                var deptName = e.Department?.Name ?? "";
                sb.AppendLine($"{EscapeCsv(e.Id.ToString())},{EscapeCsv(e.FullName)},{EscapeCsv(e.Email)},{EscapeCsv(e.Salary.ToString())},{EscapeCsv(deptName)}");
            }

            var csvBytes = Encoding.UTF8.GetBytes(sb.ToString());
            return File(csvBytes, "text/csv", "employees.csv");
        }

        // GET: api/reports/employees/pdf
        [HttpGet("employees/pdf")]
        public async Task<IActionResult> DownloadEmployeesPdf()
        {
            var idClaim = User.FindFirst("id")?.Value;
            if (!int.TryParse(idClaim, out var userId))
                return Unauthorized();

            var employees = await _db.Employees
                .Include(e => e.Department)
                .Where(e => e.UserId == userId)
                .ToListAsync();

            // Create simple PDF using PdfSharpCore
            using var ms = new MemoryStream();
            using (var document = new PdfDocument())
            {
                var page = document.AddPage();
                page.Size = PdfSharpCore.PageSize.A4;
                var gfx = XGraphics.FromPdfPage(page);
                var yStart = 40;
                var x = 40;
                var lineHeight = 18;
                var fontTitle = new XFont("Arial", 16, XFontStyle.Bold);
                var fontHeader = new XFont("Arial", 11, XFontStyle.Bold);
                var font = new XFont("Arial", 10, XFontStyle.Regular);

                gfx.DrawString("Employee Report", fontTitle, XBrushes.Black, new XRect(0, 10, page.Width, 30), XStringFormats.TopCenter);

                // header row
                gfx.DrawString("Id", fontHeader, XBrushes.Black, new XRect(x, yStart, 40, lineHeight), XStringFormats.TopLeft);
                gfx.DrawString("Full Name", fontHeader, XBrushes.Black, new XRect(x + 40, yStart, 200, lineHeight), XStringFormats.TopLeft);
                gfx.DrawString("Email", fontHeader, XBrushes.Black, new XRect(x + 250, yStart, 200, lineHeight), XStringFormats.TopLeft);
                gfx.DrawString("Salary", fontHeader, XBrushes.Black, new XRect(x + 450, yStart, 80, lineHeight), XStringFormats.TopLeft);
                yStart += lineHeight;

                foreach (var e in employees)
                {
                    // if page full, add new page
                    if (yStart + lineHeight > page.Height - 40)
                    {
                        page = document.AddPage();
                        gfx = XGraphics.FromPdfPage(page);
                        yStart = 40;
                    }

                    gfx.DrawString(e.Id.ToString(), font, XBrushes.Black, new XRect(x, yStart, 40, lineHeight), XStringFormats.TopLeft);
                    gfx.DrawString(Truncate(e.FullName, 30), font, XBrushes.Black, new XRect(x + 40, yStart, 200, lineHeight), XStringFormats.TopLeft);
                    gfx.DrawString(Truncate(e.Email, 28), font, XBrushes.Black, new XRect(x + 250, yStart, 200, lineHeight), XStringFormats.TopLeft);
                    gfx.DrawString(e.Salary.ToString("F2"), font, XBrushes.Black, new XRect(x + 450, yStart, 80, lineHeight), XStringFormats.TopLeft);
                    yStart += lineHeight;
                }

                document.Save(ms, false);
            }

            var pdfBytes = ms.ToArray();
            return File(pdfBytes, "application/pdf", "employees.pdf");
        }

        private static string EscapeCsv(string? field)
        {
            if (string.IsNullOrEmpty(field)) return "";
            var needsQuotes = field.Contains(',') || field.Contains('"') || field.Contains('\n') || field.Contains('\r');
            var escaped = field.Replace("\"", "\"\"");
            return needsQuotes ? $"\"{escaped}\"" : escaped;
        }

        private static string Truncate(string input, int max)
        {
            if (string.IsNullOrEmpty(input)) return "";
            return input.Length <= max ? input : input.Substring(0, max - 3) + "...";
        }
    }
}
