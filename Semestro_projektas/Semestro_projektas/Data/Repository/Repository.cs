using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Internal;
using Semestro_projektas.Models;

namespace Semestro_projektas.Data.Repository
{
    public class Repository : IRepository
    {


        private AppDbContext _ctx;

        public Repository(AppDbContext ctx)
        {
            _ctx = ctx;
        }


        public void AddPost(Post post)
        {
            _ctx.Posts.Add(post);  
        }

        public List<Post> GetAllPosts()
        {
            return _ctx.Posts.ToList();
        }

        public Post GetPost(int id)
        {
            return _ctx.Posts.FirstOrDefault(p => p.Id == id);
        }

        public void RemovePost(int id)
        {
            _ctx.Posts.Remove(GetPost(id));
        }

        public void UpdatePost(Post post)
        {
            _ctx.Posts.Update(post);
        }


        public async Task<bool> SaveChangesAsync()
        {

            if (await _ctx.SaveChangesAsync() > 0) {
                return true;
            }
            return false;
        }

         public bool RegisterUser(User user, string password, UserManager<User> userManager,
             RoleManager<IdentityRole> roleManager) {
             //var userManager = UserManager<User>;
             //var roleManager = RoleManager<IdentityRole>;

             _ctx.Database.EnsureCreated();

             var userRole = new IdentityRole("ChatUser");
             if (!_ctx.Roles.Any(r => r == userRole))
             {
                 //sukurti role
                 roleManager.CreateAsync(userRole).GetAwaiter().GetResult();


             }

            if (!_ctx.Users.Any(u => u.UserName == user.UserName))
            {

                var result = userManager.CreateAsync(user, password)
                   .GetAwaiter().GetResult();

                //prideti vartotojui role
                userManager.AddToRoleAsync(user, userRole.Name).GetAwaiter().GetResult();

                return true;

            }
            else {
                
                return false;
            }
         }
    }
}
