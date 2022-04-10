using Sue3.SUM.Model.Authentication;
using Sue3.SUM.Model.Components.Accessories;
using Sue3.SUM.Model.Components.Descriptive;
using Sue3.SUM.Model.Components.Physical;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
namespace Sue3.Web.Blazor.Data
{
    //This class is used to call the backend's API
    //each login has its own instance of this
    public class SueApiService
    {
        public string AuthToken { get; set; }//authorization token of user
        public string UserId { get; set; }//userId of user
        public bool LoggedIn => AuthToken != null && AuthToken != string.Empty;//if the auth token is not set, user should not be logged in.
        public Model LastAccessedModel { get; set; }//This is used to check if the the model in the modelhandler was modified.
        public async Task<T2> Post<T1, T2>(HttpClient httpClient, string url, T1 postData) // A generic method to handle post http calls
        {
            httpClient.DefaultRequestHeaders.Remove("x-logintoken");
            httpClient.DefaultRequestHeaders.Add("x-logintoken", AuthToken);
            var options = new System.Text.Json.JsonSerializerOptions() // options are used like this to make the json serializer a little bit more leniant, and to provide custom converters for rules and dictionaries.
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = null,
                IgnoreNullValues = true,
            };
            options.Converters.Add(DictionaryJsonConverterFactory.Default);
            options.Converters.Add(new SpecialDoubleJsonConverter());
            options.Converters.Add(new ExplicitRuleJsonConverter());
            var response = await httpClient.PostAsJsonAsync(url, postData, options);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<T2>(options);
            }
            else
            {
                throw new HttpRequestException();
            }
        }
        public async Task Post<T1>(HttpClient httpClient, string url, T1 postData) // A generic method to handle post http calls which return void
        {
            httpClient.DefaultRequestHeaders.Remove("x-logintoken");
            httpClient.DefaultRequestHeaders.Add("x-logintoken", AuthToken);
            var options = new System.Text.Json.JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = null,
                IgnoreNullValues = true,
            };
            options.Converters.Add(DictionaryJsonConverterFactory.Default);
            options.Converters.Add(new SpecialDoubleJsonConverter());
            options.Converters.Add(new ExplicitRuleJsonConverter());
            var response = await httpClient.PostAsJsonAsync(url, postData, options);
            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException();
            }
        }

        public async Task<T1> Get<T1>(HttpClient httpClient, string url)// A generic method to handle get http calls
        {
            httpClient.DefaultRequestHeaders.Remove("x-logintoken");
            httpClient.DefaultRequestHeaders.Add("x-logintoken", AuthToken);
            var options = new System.Text.Json.JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = null,
                IgnoreNullValues = true,
            };
            options.Converters.Add(DictionaryJsonConverterFactory.Default);
            options.Converters.Add(new SpecialDoubleJsonConverter());
            options.Converters.Add(new ExplicitRuleJsonConverter());
            return await httpClient.GetFromJsonAsync<T1>(url, options);
        }
        public async Task<OperationResult> PostRequestPasswordChange(HttpClient httpClient, string userId, string email)
        {
            return await Post<string, OperationResult>(httpClient, $"usermanagement/change-password-request?userId={userId}", email);
        }
        public async Task<OperationResult> PostPasswordChange(HttpClient httpClient,string newPassword, string passwordToken)
        {
            return await Post<string, OperationResult>(httpClient, $"usermanagement/change-password?newPassword={newPassword}", passwordToken);
        }
        public async Task<OperationResult> PostSaveUser(HttpClient httpClient, User user)
        {
            return await Post<User, OperationResult>(httpClient, $"usermanagement/save-user", user);
        }
        public async Task<LoginResult> PostLogin(HttpClient httpClient, Login login)
        {

            var loginResult = await Post<Login, LoginResult>(httpClient, "usermanagement/login", login);
            if (!loginResult.IsError)
            {
                AuthToken = loginResult.AuthorizationToken;
                UserId = loginResult.UserId;
            }
            return loginResult;
        }
        public async Task<bool> GetSuperUser(HttpClient httpClient)
        {
            return await Get<bool>(httpClient, "usermanagement/get-super-user");
        }
        public async Task<List<Model>> GetModels(HttpClient httpClient)
        {
            return await Get<List<Model>>(httpClient, "model/get-models");
        }
        public async Task<Model> GetModel(HttpClient httpClient, Guid modelGuid)
        {
            LastAccessedModel = await Get<Model>(httpClient, $"model/get-model-by-guid?modelGuid={modelGuid}");
            var options = new System.Text.Json.JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = null,
                IgnoreNullValues = true,
            };
            options.Converters.Add(DictionaryJsonConverterFactory.Default);
            options.Converters.Add(new SpecialDoubleJsonConverter());
            options.Converters.Add(new ExplicitRuleJsonConverter());
            var modelstring = JsonSerializer.Serialize(LastAccessedModel, options);//this is an easy way to make a deep copy.
            var model = JsonSerializer.Deserialize<Model>(modelstring, options);
            return model;
        }
        public async Task<Guid> GetCopyModel(HttpClient httpClient, Guid modelGuid, bool exportRules, bool exportSamples)
        {
            return await Get<Guid>(httpClient, $"model/copy-model?modelGuid={modelGuid}&version=-1&exportRules={exportRules}&exportSamples={exportSamples}");
        }
        public async Task<OperationResult> GetDeleteModel(HttpClient httpClient, Guid modelGuid)
        {
            return await Get<OperationResult>(httpClient, $"model/delete-model?modelGuid={modelGuid}");
        }
        public async Task<string> GetExportModel(HttpClient httpClient, Guid modelGuid)
        {
            httpClient.DefaultRequestHeaders.Remove("x-logintoken");
            httpClient.DefaultRequestHeaders.Add("x-logintoken", AuthToken);
            var modelString = await httpClient.GetStringAsync($"model/export-binary?modelGuid={modelGuid}&version=-1");//this doesnt use the generic get, since the return is a string, not json
            return modelString;
        }
        public async Task<Guid> PostImportModel(HttpClient httpClient, string filename, string binarystring)
        {
            httpClient.DefaultRequestHeaders.Remove("x-logintoken");
            httpClient.DefaultRequestHeaders.Add("x-logintoken", AuthToken);
            var mpr = new MultipartFormDataContent();
            mpr.Add(new StringContent(binarystring), "import", filename);
            var response = await httpClient.PostAsync("model/import-model-from-binary", mpr);//this doesnt use the generic post, since the post is a multipart form, not json
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<Guid>();
            }
            else
            {
                throw new HttpRequestException();
            }
        }
        public async Task<OperationResult> PostDeduceModel(HttpClient httpClient, string modelName, string binarystring)
        {
            return await Post<string, OperationResult>(httpClient, $"model/deduce-model?modelName={modelName}", binarystring);
        }
        public async Task<OperationResult> PostCreateOrUpdateModel(HttpClient httpClient, Model model)
        {
            return await Post<Model, OperationResult>(httpClient, $"model/post-model", model);
        }
        public async Task<OperationResult> PostCreateOrUpdateParameters(HttpClient httpClient, Guid modelGuid, List<Parameter> parameters)
        {
            return await Post<List<Parameter>, OperationResult>(httpClient, $"model/post-parameters?modelGuid={modelGuid}", parameters);
        }
        public async Task<OperationResult> PostDeleteParameters(HttpClient httpClient, Guid modelGuid, List<Guid> parameterGuids)
        {
            return await Post<List<Guid>, OperationResult>(httpClient, $"model/delete-parameters?modelGuid={modelGuid}", parameterGuids);
        }
        public async Task<TrainingSampleResult> GetTrainingSample(HttpClient httpClient, Guid modelGuid)
        {
            return await Get<TrainingSampleResult>(httpClient, $"model/get-training-sample?modelGuid={modelGuid}");
        }
        public async Task<OperationResult> PostSample(HttpClient httpClient, Sample sample)
        {
            return await Post<Sample, OperationResult>(httpClient, $"model/post-sample", sample);
        }
        public async Task<ValidationResult> PostBulkValidateData(HttpClient httpClient, Guid modelGuid, string csvString)
        {
            return await Post<string, ValidationResult>(httpClient, $"model/bulk-validate-data?modelGuid={modelGuid}&version=-1", csvString);
        }
        public async Task<OperationResult> PostBulkImportData(HttpClient httpClient, Guid modelGuid, string csvString)
        {
            return await Post<string, OperationResult>(httpClient, $"model/bulk-import-data?modelGuid={modelGuid}&version=-1", csvString);
        }
        public async Task<string> GenerateTrainingSampleCSV(HttpClient httpClient, Guid modelGuid, int nSamples)
        {
            httpClient.DefaultRequestHeaders.Remove("x-logintoken");
            httpClient.DefaultRequestHeaders.Add("x-logintoken", AuthToken);
            var trainingSamplesCSV = await httpClient.GetStringAsync($"model/generate-training-csv?modelGuid={modelGuid}&nSamples={nSamples}");//this doesnt use the generic get, since the return is a string, not json
            return trainingSamplesCSV;
        }
        public async Task<OperationResult> GetDeleteRule(HttpClient httpClient, Guid ruleGuid, Guid modelGuid)
        {
            return await Get<OperationResult>(httpClient, $"model/delete-rule?explicitRuleGuid={ruleGuid}&modelGuid={modelGuid}");
        }
        public async Task<OperationResult> PostRelationship(HttpClient httpClient, RelationshipRule relationshipRule)
        {
            return await Post<RelationshipRule, OperationResult>(httpClient, $"model/post-relationship", relationshipRule);
        }
        public async Task<OperationResult> PostExplicitRule(HttpClient httpClient, ExplicitRule explicitRule)
        {
            return await Post<ExplicitRule, OperationResult>(httpClient, $"model/post-rule", explicitRule);
        }
        public async Task<OperationResult> TrainModel(HttpClient httpClient, Guid modelGuid, bool eraseUserWeights)
        {
            return await Get<OperationResult>(httpClient, $"model/train-model?modelGuid={modelGuid}&eraseuserweights={eraseUserWeights}");
        }
        public async Task<Sample> GetNewQuery(HttpClient httpClient, Guid modelGuid)
        {
            return await Get<Sample>(httpClient, $"model/get-new-testing-sample?modelGuid={modelGuid}");
        }
        public async Task<PredictedQuery> PostAskSue(HttpClient httpClient, Sample sample)
        {
            return await Post<Sample, PredictedQuery>(httpClient, $"model/ask", sample);
        }
        public async Task PostChangeSampleOutputTemp(HttpClient httpClient, LogAndCalibrateViewModel lcv)
        {
            await Post(httpClient, "model/change-sample-output", lcv);
        }
        public async Task<OperationResult> PostSaveCalibration(HttpClient httpClient, LogAndCalibrateViewModel lcv, bool eraseUserWeights)
        {
            return await Post<LogAndCalibrateViewModel, OperationResult>(httpClient, $"model/save-calibration?eraseUserWeights={eraseUserWeights}", lcv);
        }
        public async Task<OperationResult> PostBulkSampleSaveAndTrain(HttpClient httpClient, List<Sample> samples, bool eraseUserWeights)
        {
            return await Post<List<Sample>, OperationResult>(httpClient, $"model/bulk-save-train?eraseUserWeights={eraseUserWeights}", samples);
        }
        public async Task<ValidationResult> PostBulkValidateQueries(HttpClient httpClient, Guid modelGuid, string csvString)
        {
            return await Post<string, ValidationResult>(httpClient, $"model/bulk-validate-queries?modelGuid={modelGuid}&version=-1", csvString);
        }
        public async Task<bool> PostBulkPrepareQueries(HttpClient httpClient, Guid modelGuid, string csvString)
        {
            return await Post<string, bool>(httpClient, $"model/bulk-prepare-queries?modelGuid={modelGuid}&version=-1", csvString);
        }
        public async Task<OperationResult> GetBulkPrepareQueriesResult(HttpClient httpClient)
        {
            return await Get<OperationResult>(httpClient, $"model/bulk-prepare-queries-result");
        }
        public async Task<double> GetPercentage(HttpClient httpClient, AsynchronousOperationManager.AsynchronousOperationType processType)
        {
            return await Get<double>(httpClient, $"model/get-percentage?processType={processType}");
        }
        public async Task<LogAndCalibrateViewModel> PostCheckUncalibratedSamples(HttpClient httpClient, int currentPage, int itemsPerPage, Guid modelGuid, List<ParameterFilter> filters)
        {
            return await Post<List<ParameterFilter>, LogAndCalibrateViewModel>(httpClient, $"model/post-check-uncalibrated-samples?currentPage={currentPage}&itemsPerPage={itemsPerPage}&modelGuid={modelGuid}", filters);
        }
        public async Task<IEnumerable<PredictedQuery>> PostCheckCalibratedSamples(HttpClient httpClient, int currentPage, int itemsPerPage,  LogAndCalibrateViewModel dto)
        {
            return await Post<LogAndCalibrateViewModel, IEnumerable<PredictedQuery>>(httpClient, $"model/post-check-calibration-samples?currentPage={currentPage}&itemsPerPage={itemsPerPage}", dto);
        }
        public async Task<bool> PostCalculateMFE(HttpClient httpClient, Guid modelGuid)
        {
            return await Post<int, bool>(httpClient, $"model/calculate-MFE?modelGuid={modelGuid}", -1);//-1 is sending version as -1 in body.
        }
        public async Task<double> GetMFEScore(HttpClient httpClient)
        {
            return await Get<double>(httpClient, $"model/retrieve-MFE");
        }
        public async Task<OperationResult> PostTemporaryCalibration(HttpClient httpClient, LogAndCalibrateViewModel dto)
        {
            return await Post<LogAndCalibrateViewModel, OperationResult>(httpClient, $"model/post-calibration", dto);
        }
        public async Task PostDeleteSampleTemp(HttpClient httpClient, Guid modelGuid, int sampleId)
        {
            await Post(httpClient, $"model/delete-sample?modelGuid={modelGuid}&sampleId={sampleId}", -1);// -1 in the body does not matter. It simply makes calling easier.
        }
        public async Task PostDeleteAllSampleTemp(HttpClient httpClient, Model model)
        {
            await Post(httpClient, $"model/delete-all-samples", model);
        }
        public async Task PostDeleteAllSampleFilteredTemp(HttpClient httpClient, Guid modelGuid,List<ParameterFilter> filters)
        {
            await Post(httpClient, $"model/delete-all-samples-filtered?modelGuid={modelGuid}", filters);
        }
        public async Task PostUndoAllChanges(HttpClient httpClient, Guid modelGuid)
        {
            await Post(httpClient, $"model/undo-changes?modelGuid={modelGuid}", -1);//-1 is sending version as -1 in body.
        }
        public async Task<bool> GetExportSamples(HttpClient httpClient, Guid modelGuid, bool includePredictions, bool useAllowIdenticalPoint)
        {
            return await Get<bool>(httpClient, $"model/export-samples?modelGuid={modelGuid}&includePredictions={includePredictions}&useAllowIdenticalPoint={useAllowIdenticalPoint}");
        }
        public async Task<string> RetrieveExportSamples(HttpClient httpClient)
        {
            httpClient.DefaultRequestHeaders.Remove("x-logintoken");
            httpClient.DefaultRequestHeaders.Add("x-logintoken", AuthToken);
            var samplesString = await httpClient.GetStringAsync($"model/retrieve-samples");//this doesnt use the generic get, since the return is a string, not json
            return samplesString;
        }
        public async Task PostCancelAllOperations(HttpClient httpClient)
        {
            await Post(httpClient, $"model/cancel-all-operations", -1);//-1 is not needed, only for convenience.
        }
        public async Task PostCancelCalculation(HttpClient httpClient, AsynchronousOperationManager.AsynchronousOperationType asynchronousOperationType)
        {
            await Post(httpClient, $"model/cancel-calculation", asynchronousOperationType);
        }
        public async Task<OperationResult> PostRestoreVersion(HttpClient httpClient, Guid modelGuid, int version)
        {
            return await Post<int, OperationResult>(httpClient, $"model/restore?modelGuid={modelGuid}", version);
        }
        public void LogOut()
        {
            AuthToken = null;
        }
    }
}
