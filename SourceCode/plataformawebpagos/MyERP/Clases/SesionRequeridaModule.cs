using System;
using System.Web;
using System.Web.SessionState;

namespace Plataforma.Clases
{
    /// <summary>
    /// Exige sesión iniciada para todo lo que no es público: páginas .aspx, sus WebMethods
    /// (Pagina.aspx/Metodo), handlers .ashx y los documentos de clientes en Uploads.
    /// Se registra en Web.config. Las páginas tienen su propia validación, pero comparan
    /// contra "" y sin sesión el valor es null, así que no la detectan.
    /// </summary>
    public class SesionRequeridaModule : IHttpModule
    {
        // Páginas que se pueden abrir sin iniciar sesión (rutas en minúsculas).
        private static readonly string[] PaginasPublicas =
        {
            "~/pages/login.aspx",
            "~/pages/logout.aspx",
            "~/pages/home.aspx",
            "~/pages/aboutus.aspx",
            "~/pages/faqs.aspx",
            "~/pages/noticeofprivacy.aspx",
            "~/pages/termsandconditions.aspx",
            "~/pages/tutorials.aspx"
        };

        // Carpetas con documentos de clientes que solo puede ver un usuario con sesión.
        private static readonly string[] CarpetasPrivadas =
        {
            "~/uploads/",
            "~/pages/uploads/"
        };

        public void Init(HttpApplication app)
        {
            app.PostMapRequestHandler += AlAsignarHandler;
            // En AcquireRequestState la sesión ya está cargada y los WebMethods todavía no se ejecutan.
            app.AcquireRequestState += AlCargarSesion;
        }

        public void Dispose()
        {
        }

        // Los archivos estáticos y los .ashx no cargan la sesión por sí solos;
        // se pide en modo solo lectura para poder revisar si hay usuario.
        private static void AlAsignarHandler(object sender, EventArgs e)
        {
            HttpContext context = ((HttpApplication)sender).Context;

            if (RequiereSesion(context.Request) && !(context.Handler is IRequiresSessionState))
            {
                context.SetSessionStateBehavior(SessionStateBehavior.ReadOnly);
            }
        }

        private static void AlCargarSesion(object sender, EventArgs e)
        {
            HttpApplication app = (HttpApplication)sender;
            HttpContext context = app.Context;

            if (!RequiereSesion(context.Request) || TieneSesion(context))
            {
                return;
            }

            string paginaLogin = VirtualPathUtility.ToAbsolute("~/pages/Login.aspx");
            bool esPagina = context.Request.AppRelativeCurrentExecutionFilePath.EndsWith(".aspx", StringComparison.OrdinalIgnoreCase)
                && string.IsNullOrEmpty(context.Request.PathInfo);

            if (esPagina)
            {
                context.Response.Redirect(paginaLogin, false);
            }
            else
            {
                // WebMethods, .ashx y archivos: 401 para que general.js mande al login.
                context.Response.Clear();
                context.Response.StatusCode = 401;
                context.Response.ContentType = "application/json";
                context.Response.Write("{\"Message\":\"La sesión expiró. Inicia sesión de nuevo.\",\"Login\":\"" + paginaLogin + "\"}");
            }

            app.CompleteRequest();
        }

        private static bool RequiereSesion(HttpRequest request)
        {
            string ruta = request.AppRelativeCurrentExecutionFilePath.ToLowerInvariant();

            foreach (string carpeta in CarpetasPrivadas)
            {
                if (ruta.StartsWith(carpeta, StringComparison.Ordinal))
                {
                    return true;
                }
            }

            bool esCodigo = ruta.EndsWith(".aspx", StringComparison.Ordinal)
                || ruta.EndsWith(".ashx", StringComparison.Ordinal)
                || ruta.EndsWith(".asmx", StringComparison.Ordinal);

            return esCodigo && Array.IndexOf(PaginasPublicas, ruta) < 0;
        }

        private static bool TieneSesion(HttpContext context)
        {
            return context.Session != null
                && !string.IsNullOrEmpty(context.Session["id_usuario"] as string);
        }
    }
}
