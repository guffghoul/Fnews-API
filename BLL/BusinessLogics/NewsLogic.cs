﻿using BLL.Helpers;
using BLL.Interfaces;
using BLL.Models;
using BLL.Models.NewsModels;
using DAL.UnitOfWorks;
using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography.X509Certificates;

namespace BLL.BusinessLogics
{
    public class NewsLogic : INewsLogic
    {
        private readonly FnewsContext _context;

        private readonly IUnitOfWork _unitOfWork;
        public NewsLogic(IUnitOfWork unitOfWork, FnewsContext context)
        {
            _unitOfWork = unitOfWork;
            _context = context;
        }
        public bool CreateNews(NewsViewModel newsModel)
        {
            bool check = false;
            if(newsModel != null)
            {
                try
                {
                    News news = new News()
                    {
                       
                        NewsTitle = newsModel.NewsTitle,
                        NewsContent = newsModel.NewsContent,
                        DayOfPost = newsModel.DayOfPost,
                        ChannelId = newsModel.ChannelId,
                        LinkImage = newsModel.LinkImage,
                        IsActive =  true,
                    };
                    _unitOfWork.GetRepository<News>().Insert(news);
                    _unitOfWork.Commit();
                    check = true;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                   
                }
               

            }
            return check;
        }

        public bool DeleteNews(int id)
        {
            bool check = false;
            News news = _unitOfWork.GetRepository<News>().FindById(id);
            if (news != null)
            {
                news.IsActive = false;
                _unitOfWork.GetRepository<News>().Update(news);
                _unitOfWork.Commit();
                check = true;
            }
            return check;
        }

        public IQueryable<News> GetAllNews()
        {
            IQueryable<News> news = _unitOfWork.GetRepository<News>().GetAll().Where(n => n.IsActive == true)
                                                    .Include(x => x.NewsTag).ThenInclude(y => y.Tag)
                                                    .Include(x => x.Channel).OrderByDescending(n => n.DayOfPost);
                                
            return news;
        }
        
        

        public News GetNewsById(int id)
        {
            News news = _unitOfWork.GetRepository<News>().FindById(id);
            return news;
        }

        public List<NewsViewModel> SearchNewsByTitle(string title, PagingModel pagingModel)
        {
            IEnumerable<NewsViewModel> newsModel = _unitOfWork
                .GetRepository<News>()
                .GetAll()
                .Where(a => a.NewsTitle.Contains(title))
                .Select(a => new NewsViewModel
                {
                    NewsId = a.NewsId,
                    NewsTitle = a.NewsTitle,
                    NewsContent = a.NewsContent
                });
            if(newsModel != null)
            {
                var paging = new Paging();
                if(title == null)
                {
                    title = "";
                }
                var searchTitle = title.ToLower();
                var result = new List<NewsViewModel>();
                result = newsModel
                    .Where(a => a.NewsTitle.ToLower().Contains(title))
                    .OrderBy(a => a.NewsTitle)
                    .Skip(paging.SkipItem(pagingModel.PageNumber, pagingModel.PageSize))
                    .Take(pagingModel.PageSize)
                    .ToList();
                if(result != null)
                {
                    return result;
                }


            }
            return null;

        }

        public bool UpdateNews(News news)
        {
            bool check = false;
            if(news != null)
            {
                _unitOfWork.GetRepository<News>().Update(news);
                _unitOfWork.Commit();
                check = true;
                
            }
            return check;

        }


   
    }
}
