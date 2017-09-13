using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ValidationSample;
using AutoMapper;
using ValidationDAL;
namespace ValidationSample.Repository
{
    public class ValidationMapper<E,M>
        where E :class
        where M :class
    {
        public ValidationMapper()
        {
            Mapper.CreateMap<TestCase, Models.TestCase>();
            Mapper.CreateMap<Models.TestCase, TestCase>();
            Mapper.CreateMap<TestSuite, Models.TestSuite>();
            Mapper.CreateMap<Models.TestSuite, TestSuite>();
            Mapper.CreateMap<Event, Models.Event>();
            Mapper.CreateMap<Models.Event, Event>();
            Mapper.CreateMap<TestCaseLog, Models.TestCaseLog>();
            Mapper.CreateMap<Models.TestCaseLog, TestCaseLog>();
        }
        public M Translate(E obj)
        {
            return Mapper.Map<E, M>(obj);
        }
    }
}