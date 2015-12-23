using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.DirectoryServices.AccountManagement;

namespace JVBMG.Util
{
    public class ActiveDirectory
    {
        public string Name { get; internal set; }
        public string Credential { get; internal set; }
        public string Email { get; internal set; }
        public string WindowsUser { get; internal set; }

        private ActiveDirectory()
        {
            Name = string.Empty;
            Credential = string.Empty;
            Email = string.Empty;
            WindowsUser = string.Empty;
        }

        public static ActiveDirectory Load(string windowsUser)
        {
            PrincipalContext pc = new PrincipalContext(ContextType.Domain);
            var loggedUser = UserPrincipal.FindByIdentity(pc, IdentityType.SamAccountName, windowsUser);
            if (loggedUser == null)
                return null;

            return new ActiveDirectory()
            {
                Name = loggedUser.DisplayName,
                Email = loggedUser.EmailAddress,
                Credential = loggedUser.Description,
                WindowsUser = windowsUser
            };
        }

        public static List<ActiveDirectory> LoadUsers(string domain)
        {
            PrincipalContext pc = new PrincipalContext(ContextType.Domain, domain);
            var search = new System.DirectoryServices.AccountManagement.PrincipalSearcher(new UserPrincipal(pc));
            search.QueryFilter.Description = "*";
            ((UserPrincipal)search.QueryFilter).EmailAddress = "*";
            return search.FindAll().Where(d => d != null && d is UserPrincipal).Select(UsuarioLogado =>
                new ActiveDirectory()
                {
                    Name = UsuarioLogado.DisplayName,
                    Email = ((UserPrincipal)UsuarioLogado).EmailAddress,
                    Credential = UsuarioLogado.Description,
                    WindowsUser = UsuarioLogado.Name,
                }).ToList();
        }

        public static ActiveDirectory Load(string domain, string credential)
        {
            PrincipalContext pc = new PrincipalContext(ContextType.Domain, domain);
            var Busca = new System.DirectoryServices.AccountManagement.PrincipalSearcher();
            Busca.QueryFilter = new UserPrincipal(pc);
            Busca.QueryFilter.Description = credential;
            var Resultado = Busca.FindOne();
            if (null != Resultado)
            {
                var UsuarioLogado = Resultado as UserPrincipal;
                return new ActiveDirectory()
                {
                    Name = UsuarioLogado.DisplayName,
                    Email = UsuarioLogado.EmailAddress,
                    Credential = UsuarioLogado.Description,
                    WindowsUser = UsuarioLogado.Name
                };
            }
            return null;
        }

        public static ActiveDirectory Load(string domain, string name)
        {
            PrincipalContext pc = new PrincipalContext(ContextType.Domain, domain);
            var Busca = new System.DirectoryServices.AccountManagement.PrincipalSearcher();
            Busca.QueryFilter = new UserPrincipal(pc);
            Busca.QueryFilter.DisplayName = name;
            var Resultado = Busca.FindOne();
            if (null != Resultado)
            {
                var UsuarioLogado = Resultado as UserPrincipal;
                return new ActiveDirectory()
                {
                    Name = UsuarioLogado.DisplayName,
                    Email = UsuarioLogado.EmailAddress,
                    Credential = UsuarioLogado.Description,
                    WindowsUser = UsuarioLogado.Name
                };
            }
            return null;
        }
    }
}
