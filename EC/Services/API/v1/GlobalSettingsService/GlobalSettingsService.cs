using EC.Models.API.v1.GlobalSettings;
using EC.Models.Database;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace EC.Services.API.v1.GlobalSettingsService
{
    internal class GlobalSettingsService : ServiceBase<global_settings>
    {
        public async Task<int> CreateAsync(CreateGlobalSettingsModel createGlobalSettingsModel, bool isCC)
        {
            if (createGlobalSettingsModel == null)
            {
                throw new ArgumentNullException(nameof(createGlobalSettingsModel));
            }
            global_settings newGlobal_Settings = _set.Add(createGlobalSettingsModel, global_setting =>
            {
                global_setting.client_id = createGlobalSettingsModel.client_id;
                global_setting.application_name = createGlobalSettingsModel.application_name;
                global_setting.custom_logo_path = createGlobalSettingsModel.custom_logo_path;
                global_setting.header_color_code = createGlobalSettingsModel.header_color_code;
                global_setting.header_links_color_code = createGlobalSettingsModel.header_links_color_code;
            });

            await _appContext
                .SaveChangesAsync()
                .ConfigureAwait(false);

            return newGlobal_Settings.id;
        }
        public async Task<int> UpdateAsync(CreateGlobalSettingsModel createGlobalSettingsModel, int id)
        {
            if (createGlobalSettingsModel == null)
            {
                throw new ArgumentNullException(nameof(createGlobalSettingsModel));
            }

            if (id == 0)
            {
                throw new ArgumentException("The ID can't be empty.", nameof(id));
            }
            var newGlobalSetting = _appContext.global_settings.Where(global_setting => global_setting.client_id == id).FirstOrDefault();
            if (newGlobalSetting == null)
            {
                throw new ArgumentException("global setting not found.", nameof(id));
            }

            createGlobalSettingsModel.client_id = id;
            global_settings newGlobal_Settings = await _set
                .UpdateAsync(newGlobalSetting.id, createGlobalSettingsModel)
                .ConfigureAwait(false);

            await _appContext
                .SaveChangesAsync()
                .ConfigureAwait(false);
            return newGlobal_Settings.id;
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