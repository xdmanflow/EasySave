using System.Collections.Generic;

namespace EasySave.Languages
{
    public class LanguageManager
    {
        private readonly Dictionary<string, string> _strings;

        private static readonly Dictionary<string, string> English = new()
        {
            ["menu_title"]      = "=== EasySave v1.0 ===",
            ["menu_list"]       = "1. List backup jobs",
            ["menu_add"]        = "2. Add a backup job",
            ["menu_delete"]     = "3. Delete a backup job",
            ["menu_run_one"]    = "4. Run a single backup job",
            ["menu_run_all"]    = "5. Run all backup jobs",
            ["menu_language"]   = "6. Change language",
            ["menu_exit"]       = "0. Exit",
            ["menu_choice"]     = "Your choice: ",
            ["no_jobs"]         = "No backup jobs configured yet.",
            ["job_number"]      = "Job #{0}: {1} | {2} -> {3} | {4}",
            ["add_name"]        = "Job name: ",
            ["add_source"]      = "Source directory: ",
            ["add_target"]      = "Target directory: ",
            ["add_type"]        = "Type (1=Full / 2=Differential): ",
            ["add_success"]     = "Job '{0}' added.",
            ["add_max"]         = "Maximum of 5 jobs reached.",
            ["add_duplicate"]   = "A job with this name already exists.",
            ["delete_index"]    = "Job number to delete (1-{0}): ",
            ["delete_success"]  = "Job '{0}' deleted.",
            ["run_index"]       = "Job number to run (1-{0}): ",
            ["run_start"]       = "Running '{0}'...",
            ["run_done"]        = "Job '{0}' completed.",
            ["run_error"]       = "Error during '{0}': {1}",
            ["run_all_none"]    = "No jobs to run.",
            ["language_prompt"] = "Language (en/fr/es): ",
            ["invalid_choice"]  = "Invalid choice.",
            ["press_enter"]     = "Press Enter to continue...",
        };

        private static readonly Dictionary<string, string> French = new()
        {
            ["menu_title"]      = "=== EasySave v1.0 ===",
            ["menu_list"]       = "1. Lister les sauvegardes",
            ["menu_add"]        = "2. Ajouter une sauvegarde",
            ["menu_delete"]     = "3. Supprimer une sauvegarde",
            ["menu_run_one"]    = "4. Lancer une sauvegarde",
            ["menu_run_all"]    = "5. Lancer toutes les sauvegardes",
            ["menu_language"]   = "6. Changer de langue",
            ["menu_exit"]       = "0. Quitter",
            ["menu_choice"]     = "Votre choix : ",
            ["no_jobs"]         = "Aucune sauvegarde configurée.",
            ["job_number"]      = "Tâche #{0} : {1} | {2} -> {3} | {4}",
            ["add_name"]        = "Nom de la tâche : ",
            ["add_source"]      = "Répertoire source : ",
            ["add_target"]      = "Répertoire cible : ",
            ["add_type"]        = "Type (1=Complète / 2=Différentielle) : ",
            ["add_success"]     = "Tâche '{0}' ajoutée.",
            ["add_max"]         = "Maximum de 5 tâches atteint.",
            ["add_duplicate"]   = "Une tâche portant ce nom existe déjà.",
            ["delete_index"]    = "Numéro de la tâche à supprimer (1-{0}) : ",
            ["delete_success"]  = "Tâche '{0}' supprimée.",
            ["run_index"]       = "Numéro de la tâche à exécuter (1-{0}) : ",
            ["run_start"]       = "Exécution de '{0}'...",
            ["run_done"]        = "Tâche '{0}' terminée.",
            ["run_error"]       = "Erreur lors de '{0}' : {1}",
            ["run_all_none"]    = "Aucune tâche à exécuter.",
            ["language_prompt"] = "Langue (en/fr/es) : ",
            ["invalid_choice"]  = "Choix invalide.",
            ["press_enter"]     = "Appuyez sur Entrée pour continuer...",
        };

        private static readonly Dictionary<string, string> Spanish = new()
        {
            ["menu_title"]      = "=== EasySave v1.0 ===",
            ["menu_list"]       = "1. Listar copias de seguridad",
            ["menu_add"]        = "2. Agregar una copia de seguridad",
            ["menu_delete"]     = "3. Eliminar una copia de seguridad",
            ["menu_run_one"]    = "4. Ejecutar una copia de seguridad",
            ["menu_run_all"]    = "5. Ejecutar todas las copias de seguridad",
            ["menu_language"]   = "6. Cambiar idioma",
            ["menu_exit"]       = "0. Salir",
            ["menu_choice"]     = "Tu opcion: ",
            ["no_jobs"]         = "No hay copias de seguridad configuradas.",
            ["job_number"]      = "Tarea n. {0}: {1} | {2} -> {3} | {4}",
            ["add_name"]        = "Nombre de la tarea: ",
            ["add_source"]      = "Directorio de origen: ",
            ["add_target"]      = "Directorio de destino: ",
            ["add_type"]        = "Tipo (1=Completa / 2=Diferencial): ",
            ["add_success"]     = "Tarea '{0}' agregada.",
            ["add_max"]         = "Se alcanzo el maximo de 5 tareas.",
            ["add_duplicate"]   = "Ya existe una tarea con ese nombre.",
            ["delete_index"]    = "Numero de tarea a eliminar (1-{0}): ",
            ["delete_success"]  = "Tarea '{0}' eliminada.",
            ["run_index"]       = "Numero de tarea a ejecutar (1-{0}): ",
            ["run_start"]       = "Ejecutando '{0}'...",
            ["run_done"]        = "Tarea '{0}' completada.",
            ["run_error"]       = "Error durante '{0}': {1}",
            ["run_all_none"]    = "No hay tareas para ejecutar.",
            ["language_prompt"] = "Idioma (en/fr/es): ",
            ["invalid_choice"]  = "Opcion invalida.",
            ["press_enter"]     = "Presiona Enter para continuar...",
        };

        public LanguageManager(string code = "en")
        {
            _strings = code switch
            {
                "fr" => French,
                "es" => Spanish,
                _    => English
            };
        }

        public string Get(string key, params object[] args)
        {
            if (!_strings.TryGetValue(key, out string? val)) return $"[{key}]";
            return args.Length > 0 ? string.Format(val, args) : val;
        }
    }
}
