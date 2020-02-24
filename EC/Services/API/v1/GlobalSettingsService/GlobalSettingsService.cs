using EC.Models.API.v1.GlobalSettings;
using EC.Models.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Castle.Components.DictionaryAdapter;
using EC.Models.API.v1.Client;

namespace EC.Services.API.v1.GlobalSettingsService
{
    internal class GlobalSettingsService : ServiceBase<global_settings>
    {
        public GlobalSettingsModel getByClientId(int client_id)
        {
            var globalSetting = _set.Where(gl => gl.client_id == client_id).FirstOrDefault();
            if(globalSetting!=null)
            {
        return new GlobalSettingsModel()
        {
                    customLogoPath = "your logo path",// globalSetting.custom_logo_path,
                    headerColorCode = "#8ac858", // globalSetting.header_color_code,
                    headerLinksColorCode = "#fff"// globalSetting.header_links_color_code
                };
            }
            return null;
        }

        public async Task CreateAsync(List<CreateClientModel> createClientModel)
        {
            List<global_settings> globalSettings = new EditableList<global_settings>();

            foreach (var client in createClientModel)
            {
                if(client.GlobalSettings == null)
                    throw new ArgumentNullException(nameof(CreateGlobalSettingsModel));

                globalSettings.Add(new global_settings()
                {
                    client_id = client.Id,
                    custom_logo_path = client.GlobalSettings.CustomLogoPath,
                    header_color_code = client.GlobalSettings.HeaderColorCode,
                    header_links_color_code = client.GlobalSettings.HeaderLinksColorCode
                });
            }

            _appContext.global_settings.AddRange(globalSettings);

            await _appContext
                .SaveChangesAsync()
                .ConfigureAwait(false);

        }

        public async Task<int> UpdateAsync(CreateGlobalSettingsModel createGlobalSettingsModel, int id)
        {

            var newGlobalSetting = _appContext.global_settings.FirstOrDefault(global_setting => global_setting.client_id == id);
            if (newGlobalSetting == null)
                throw new ArgumentException("Global setting not found.", nameof(id));

            createGlobalSettingsModel.ClientId = id;

            global_settings newGlobalSettings = await _set
                .UpdateAsync(newGlobalSetting.id, createGlobalSettingsModel)
                .ConfigureAwait(false);

            await _appContext
                .SaveChangesAsync()
                .ConfigureAwait(false);

            return newGlobalSettings.id;
        }
        public async Task<int> DeleteAsync(int id)
        {
            if (id == 0)
            {
                throw new ArgumentException("The ID can't be empty.", nameof(id));
            }

            global_settings global_Settings = await _set.FindAsync(id);
            if (global_Settings == null)
            {
                throw new ArgumentException("Global_Settings not found.", nameof(id));
            }

            return global_Settings.id;
        }
    }
}