using System;
using System.Collections.Generic;
using System.Linq;

using Psythyst;

namespace Psythyst.Core
{
    /// <summary>
    /// ProjectUnit Class.
    /// </summary>
    public class ProjectUnit<TSource, TResult> : IProjectUnit<TSource, TResult>
    {
        Dictionary<string, IGenerator<TSource, TResult>> _Generator = new Dictionary<string, IGenerator<TSource, TResult>>();
        Dictionary<string, IPostProcessor<TResult>> _PostProcessor = new Dictionary<string, IPostProcessor<TResult>>();
 
        public IProjectUnit<TSource, TResult> AddGenerator(IGenerator<TSource, TResult> Generator, bool Condition = true)
        {
            if(Condition) _Generator.Add(Generator.ToString(), Generator); return this;
        }

        public IProjectUnit<TSource, TResult> AddGeneratorCollection(IEnumerable<IGenerator<TSource, TResult>> Collection, bool Condition = true)
        {
            if(Condition) Collection.Each(x => _Generator.Add(x.ToString(), x)); return this;
        }

        public IProjectUnit<TSource, TResult> AddPostProcessor(IPostProcessor<TResult> PostProcessor, bool Condition = true)
        {
            if(Condition) _PostProcessor.Add(PostProcessor.ToString(), PostProcessor); return this;
        }

        public IProjectUnit<TSource, TResult> AddPostProcessorCollection(IEnumerable<IPostProcessor<TResult>> Collection, bool Condition = true)
        {
            if(Condition) Collection.Each(x => _PostProcessor.Add(x.ToString(), x)); return this;
        }

        public IEnumerable<TResult> RunGenerator(TSource Model, Action<IGenerator<TSource, TResult>, Exception> OnError = null)
        {
            return RunGenerator(Model, _Generator.Values, OnError);
        }

        public IEnumerable<TResult> RunPostProcessor(IEnumerable<TResult> ResultCollection, Action<IPostProcessor<TResult>, Exception> OnError = null)
        {
            return RunPostProcessor(ResultCollection, _PostProcessor.Values, OnError);
        }

        public IEnumerable<TResult> Run(TSource Model, Action<IGenerator<TSource, TResult>, Exception> OnGeneratorError = null, Action<IPostProcessor<TResult>, Exception> OnPostProcessorError = null)
        {
            return RunPostProcessor(RunGenerator(Model, OnGeneratorError), OnPostProcessorError);
        }

        public static IProjectUnit<TSource, TResult> Create()
        {
            return new ProjectUnit<TSource, TResult>();
        }

        public static IEnumerable<TResult> RunGenerator(TSource Model, IEnumerable<IGenerator<TSource, TResult>> Collection, Action<IGenerator<TSource, TResult>, Exception> OnError = null)
        {
            var _ResultList = new List<TResult>();
            var _GeneratorCollection = Collection.OrderBy(x => x.Priority);

            foreach (var Generator in _GeneratorCollection)
            {
                try 
                { 
                    var _Result = Generator.Generate(Model); 
                    _ResultList.AddRange(_Result);
                }
                catch (Exception Error) 
                {
                    OnError?.Invoke(Generator, Error);
                }
            }

            return _ResultList;
        }

        public static IEnumerable<TResult> RunPostProcessor(IEnumerable<TResult> ResultCollection, IEnumerable<IPostProcessor<TResult>> PostProcessorCollection, Action<IPostProcessor<TResult>, Exception> OnError = null)
        {
            var ResultList = new List<TResult>();
            var _PostProcessorCollection = PostProcessorCollection.OrderByDescending(x => x.Priority);
            var Current = ResultCollection; 

            foreach (var PostProcessor in _PostProcessorCollection)
            {
                try 
                { 
                    Current = PostProcessor.Process(Current); 
                }
                catch (Exception Error) 
                {
                    OnError?.Invoke(PostProcessor, Error);
                }
            }

            return Current;
        }

        public static IEnumerable<TResult> Run(TSource Model, IEnumerable<IGenerator<TSource, TResult>> GeneratorCollection, IEnumerable<IPostProcessor<TResult>> PostProcessorCollection, Action<IGenerator<TSource, TResult>, Exception> OnGeneratorError = null, Action<IPostProcessor<TResult>, Exception> OnPostProcessorError = null)
        {
            var _GeneratorResultCollection = RunGenerator(Model, GeneratorCollection, OnGeneratorError);
            var _PostProcessorResultCollection = RunPostProcessor(_GeneratorResultCollection, PostProcessorCollection, OnPostProcessorError);

            return _PostProcessorResultCollection;
        }
    }
}