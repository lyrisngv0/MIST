﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MIST.Data;
using MIST.Models;

namespace MIST.Pages.Feedbacks
{
    [Authorize(Roles = "Admin, Staff")]

    public class IndexModel : PageModel
    {
        private readonly MIST.Data.MISTDbContext _context;

        public IndexModel(MIST.Data.MISTDbContext context)
        {
            _context = context;
        }

        public IList<Feedback> Feedback { get;set; }

        public async Task OnGetAsync()
        {
            Feedback = await _context.Feedback
                .Include(f => f.User).ToListAsync();
        }
    }
}
