﻿using Brobot.Commands.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Brobot.Commands.Services
{
    public interface IRandomFactService
    {
        Task<RandomFact> GetRandomFactAsync();
    }
}
