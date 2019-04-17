using ImageServe.Models;
using ImageServe.WebModels.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ImageServe.Services.Contracts
{
  public interface ITagService
    {
        Task AddToImageAsync(Image image);

        Task UpdateDescriptionTagsAsync(Image image);
    }
}
