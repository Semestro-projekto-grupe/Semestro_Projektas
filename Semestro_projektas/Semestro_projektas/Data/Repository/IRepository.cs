using Semestro_projektas.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Semestro_projektas.Data.Repository
{
    public interface IRepository
    {

        Post GetPost(int id);
        List<Post> GetAllPosts();
        void AddPost(Post post);
        void RemovePost(int id);
        void UpdatePost(Post post);

        Task<bool> SaveChangesAsync();

    }
}
