﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using MIST.Data;
using MIST.Models;

namespace MIST.Pages.Games
{
    [Authorize(Roles = "Admin, Staff")]

    public class CreateModel : PageModel
    {
        private readonly MIST.Data.MISTDbContext context;
        private readonly IWebHostEnvironment hostingEnvironment;


        public CreateModel(MIST.Data.MISTDbContext context, IWebHostEnvironment environment)
        {
            this.hostingEnvironment = environment;
            this.context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public Game Game { get; set; }

        [BindProperty]
        public IFormFile CoverImage { set; get; }

        [BindProperty]
        public IFormFile Media { set; get; }

      

    // To protect from overposting attacks, enable the specific properties you want to bind to, for
    // more details, see https://aka.ms/RazorPagesCRUD.
    public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)

                return Page();

            if (this.CoverImage != null)
            {
                var fileName = GetUniqueName(this.CoverImage.FileName);
                var uploads = Path.Combine(hostingEnvironment.WebRootPath, "uploads");
                var filePath = Path.Combine(uploads, fileName);
                this.CoverImage.CopyTo(new FileStream(filePath, FileMode.Create));
                this.Game.CoverImageName = fileName; // Set the file name
            }

            if (this.Media != null)
            {
                var fileName2 = GetUniqueName(this.Media.FileName);
                var uploads2 = Path.Combine(hostingEnvironment.WebRootPath, "uploads");
                var filePath2 = Path.Combine(uploads2, fileName2);
                this.Media.CopyTo(new FileStream(filePath2, FileMode.Create));
                this.Game.MediaName = fileName2; // Set the file name
            }

            context.Game.Add(Game);
            if (await context.SaveChangesAsync() > 0)
            {
                // Create an auditrecord object
                var auditrecord = new AuditRecord();
                auditrecord.AuditActionType = "Created Game Record";
                auditrecord.DateTimeStamp = DateTime.Now;
                auditrecord.KeyMovieFieldID = Game.ID;
                // Get current logged-in user
                var userID = User.Identity.Name.ToString();
                auditrecord.Username = userID;

                context.AuditRecords.Add(auditrecord);
                await context.SaveChangesAsync();
            }


            return RedirectToPage("./Index");
        }
        
        private string GetUniqueName(string fileName)
        {
            fileName = Path.GetFileName(fileName);
            return Path.GetFileNameWithoutExtension(fileName)
                    + "_" + Guid.NewGuid().ToString().Substring(0, 4)
                    + Path.GetExtension(fileName);
        }
    }
}