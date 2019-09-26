export function createJobs(job, selectedOptionJobCategory) {
    var contentItems = [];
    if(selectedOptionJobCategory)
    {
      selectedOptionJobCategory.map((item, key) => 
      {         
          //       
          const jobCategory = {
                                ContentItemId: item.contentItemId,
                                ContentItemVersionId: item.contentItemVersionId,
                                ContentType: item.contentType,
                                DisplayText: item.description,
                                Latest: item.latest,
                                Published: item.published,
                                ModifiedUtc: item.modifiedUtc,
                                PublishedUtc: item.publishedUtc,
                                CreatedUtc: item.createdUtc,
                                Owner: item.owner,
                                Author: item.author,
                                JobCategory: {
                                  icon: {
                                    Paths: []
                                  },
                                  Description: {
                                    Text: item.description
                                  }
                                },
                                TitlePart: {
                                  Title: item.description
                                }
                              };
          contentItems.push(jobCategory);
      });
    }
    
    const JobsType = {
      ContentItemId: '',
      ContentItemVersionId: '',
      ContentType: 'Jobs',
      DisplayText: job.jobName,
      Latest: true,
      Published: false,
      ModifiedUtc: '',
      PublishedUtc: '',
      CreatedUtc: '',
      Owner: 'admin',
      Author: 'admin',
      Jobs: {
        Name: {
          Text: job.jobName
        },
        HashTag: {
          Text: job.jobHashTag
        },
        Keyword: {
          Text: job.jobKeyword
        },
        Link: {
          Text: job.jobLink
        },
        Description: {
          Text: job.jobDescription
        }
      },
      TitlePart: {
        Title: job.jobName
      },
      BagPart: {
        ContentItems: contentItems
      }
    }
  
    return JobsType;
  }