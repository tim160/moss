using System;
using System.Collections.Generic;

namespace EC.Core.Common
{
    public interface IBatcher
    {        
        /// <summary>
        /// Batches the items into batches of up to specified size and processes them with supplied process function and process also key for the items to return a dictionary
        /// </summary>
        /// <typeparam name="TKey">The key of model</typeparam>
        /// <typeparam name="TModel">The type to batch</typeparam>
        /// <typeparam name="TPreTransformModel">Type of pre transformed model</typeparam>
        /// <param name="items">The set of items to process</param>
        /// <param name="batchSize">The size to batch</param>
        /// <param name="processItems">A function to process the items in the batch (null takes no action)</param>
        /// <param name="keySelector">A function to select the jey for specific item</param>
        /// <returns></returns>
        Dictionary<TKey, TModel> BatchToDictionary<TKey, TModel, TPreTransformModel>(
            IEnumerable<TPreTransformModel> items,
            int batchSize,
            Func<IEnumerable<TPreTransformModel>, IEnumerable<TModel>> tranformItems,
            Func<TModel, TKey> keySelector)
            where TKey : struct
            where TModel : class
            where TPreTransformModel : class;

        /// <summary>
        /// Batches the items into batches of up to specified size and processes them with supplied process function
        /// </summary>
        /// <typeparam name="T">The type to batch</typeparam>
        /// <param name="items">The set of items to process</param>
        /// <param name="batchSize">The size to batch</param>
        /// <param name="processItems">A function to process the items in the batch (null takes no action)</param>
        IEnumerable<T> Batch<T>(
            IEnumerable<T> items,
            int batchSize,
            Func<IEnumerable<T>, IEnumerable<T>> processItems)
             where T : class;

        /// <summary>
        /// Batches the items into batches of up to specified size, transform them with supplied transform function, and processes them with supplied process function
        /// </summary>
        /// <typeparam name="TPreTransformModel">The type of the parsed item</typeparam>
        /// <typeparam name="TModel">The type of the model object for the parsed item</typeparam>
        /// <param name="preTransformItems">The set items to transform and then process</param>
        /// <param name="batchSize">The size to batch</param>
        /// <param name="tranformItems">A function that takes transforms preTransformed items into model objects (null ignores transformation)</param>
        /// <param name="processItems">A function to process transformed items (null takes no action)</param>
        IEnumerable<TModel> Batch<TPreTransformModel, TModel>(
            IEnumerable<TPreTransformModel> preTransformItems,
            int batchSize,
            Func<IEnumerable<TPreTransformModel>, IEnumerable<TModel>> tranformItems,
            Func<IEnumerable<TModel>, IEnumerable<TModel>> processItems)
            where TPreTransformModel : class
            where TModel : class;

        /// <summary>
        /// Allows the caller to process parsed items in batches.
        /// </summary>
        /// <typeparam name="TParsed">The type of the parsed item.</typeparam>
        /// <typeparam name="TModel">The type of the model object for the parsed item.</typeparam>
        /// <param name="desiredBatchSize">The desired batch size.</param>
        /// <param name="parsedItems">The entire list of parsed items to process in batches.</param>
        /// <param name="batchToExistingModels">A function that takes a batch of parsed items and returns a dictionary of existing model objects.</param>
        /// <param name="processParsedItem">An action to process a parsed item. May be null.</param>
        /// <param name="processAllParsedItems">An action to process all parsed items. May be null.</param>
        void Batch<TParsed, TModel>(int desiredBatchSize,
                                    IList<TParsed> parsedItems,
                                    Func<IEnumerable<TParsed>, IDictionary<Guid, TModel>> batchToExistingModels,
                                    Action<TParsed, IDictionary<Guid, TModel>> processParsedItem,
                                    Action<IEnumerable<TParsed>, IDictionary<Guid, TModel>> processAllParsedItems = null)
            where TParsed : class
            where TModel : class;

        /// <summary>
        /// Allows the caller to process parsed items' children in batches. Essentially it will batch together up to N
        /// parents such that the number of their children to be processed
        /// </summary>
        /// <typeparam name="TChildModel">The type of the model object for the child item.</typeparam>
        /// <typeparam name="TParentParsed">The type of the object for the parsed parent item.</typeparam>
        /// <typeparam name="TParentModel">The type of the model object for the parent item.</typeparam>
        /// <param name="desiredChildBatchSize">The desired batch size of children.</param>
        /// <param name="parentItems">The parsed parent and parent model items.</param>
        /// <param name="parsedParentToParsedChildCount">Returns the count of parsed child objects for a parsed parent item.</param>
        /// <param name="batchToExistingChildModels">A function that takes a batch of parsed parent items and returns a dictionary of existing child model objects.</param>
        /// <param name="processParent">An action to process a parsed parent item.</param>
        void BatchChildren<TChildModel, TParentParsed, TParentModel>(
                                    int desiredChildBatchSize,
                                    IList<Tuple<TParentParsed, TParentModel>> parentItems,
                                    Func<TParentParsed, int> parsedParentToParsedChildCount,
                                    Func<IEnumerable<TParentParsed>, IDictionary<Guid, TChildModel>> batchToExistingChildModels,
                                    Action<Tuple<TParentParsed, TParentModel>, IDictionary<Guid, TChildModel>> processParent)
            where TChildModel : class
            where TParentParsed : class
            where TParentModel : class;
    }
}