export function createCampaigns(brandName, brandFullName, businessAreas, brandLocation, compaignType) {

  const campaignsType = {
    ContentItemId: '',
    ContentItemVersionId: '',
    ContentType: 'Campaigns',
    DisplayText: brandName + ';' + businessAreas + ';' + brandFullName + ';' + brandLocation,//brand.Brand.BrandName.Text,
    Latest: true,
    Published: false,
    ModifiedUtc: '',
    PublishedUtc: '',
    CreatedUtc: '',
    Owner: brandName,
    Author: brandName,
    Campaigns: {
      FullName: {
        Text: brandFullName,//brand.Brand.FullName.Text
      },
      Email: {
        Text: ''
      },
      BrandName: {
        Text: brandName,//brand.Brand.BrandName.Text
      },
      BusinessAreas: {
        Text: businessAreas,//brand.Brand.BusinessAreas.Text
      },
      Phone: {
        Text: ''
      },
      Password: {
        Text: ''
      },
      Location: {
        Text: brandLocation//brand.Brand.Location.Text
      }
    },
    AutoroutePart: {
      Path: null,
      SetHomepage: false
    },
    BagPart: {
      ContentItems: [
        compaignType
      ]
    },
    TitlePart: {
      Title: brandName + ';' + businessAreas + ';' + brandFullName + ';' + brandLocation
    }
  }

  return campaignsType;
}

export function createCampaign(campaign,
  fromDate,
  toDate,
  job,
  selectedOptionLocation,
  selectedOptionInteresting,
  selectedOptionJobCategory,
  brandName,
  selectedInfluencer) {

  let locationString = '';
  var i;
  for (i = 0; i < selectedOptionLocation.length; i++) {
    locationString += selectedOptionLocation[i].value + ',';
  }

  let interestingString = '';
  var i;
  for (i = 0; i < selectedOptionInteresting.length; i++) {
    interestingString += selectedOptionInteresting[i].value + ',';
  }

  let shareLinkCost = 0;
  let postImageCost = 0;
  let videoCost = 0;
  let checkInCost = 0;
  let liveStreamCost = 0;
  let isShareLink = 0;
  let isPostImage = 0;
  let isVideo = 0;
  let isCheckIn = 0;
  let isLiveStream = 0;

  // Calculate Total Cost each
  // if (influencer.shareLink) {
  //   shareLinkCost = shareLinkCost + Number(influencer.shareLink);
  // }
  // if (influencer.postImage) {
  //   postImageCost = postImageCost + Number(influencer.postImage);
  // }
  // if (influencer.video) {
  //   videoCost = videoCost + Number(influencer.video);
  // }
  // if (influencer.checkIn) {
  //   checkInCost = checkInCost + Number(influencer.checkIn);
  // }
  // if (influencer.liveStream) {
  //   liveStreamCost = liveStreamCost + Number(influencer.liveStream);
  // }

  ///////////////////////

  // Check Cost of each job
  var i;
  for (i = 0; i < selectedOptionJobCategory.length; i++) {
    if (selectedOptionJobCategory[i].value == "Share Link") {
      isShareLink = true;
    }
    if (selectedOptionJobCategory[i].value == "Post Image") {
      isPostImage = true;
    }
    if (selectedOptionJobCategory[i].value == "Live Stream") {
      isLiveStream = true;
    }
    if (selectedOptionJobCategory[i].value == "Check In") {
      isCheckIn = true;
    }
    if (selectedOptionJobCategory[i].value == "Video") {
      isVideo = true;
    }
  }
  ///////////////////////////

  var campaignContentItems = [];

  if (selectedInfluencer) {
    const influencerLocal = {
      ContentItemId: selectedInfluencer.contentItemId,
      ContentItemVersionId: selectedInfluencer.contentItemVersionId,
      ContentType: 'Influencer',
      DisplayText: selectedInfluencer.displayText,
      Latest: true,
      Published: false,
      ModifiedUtc: selectedInfluencer.modifiedUtc,
      PublishedUtc: selectedInfluencer.publishedUtc,
      CreatedUtc: selectedInfluencer.createdUtc,
      Owner: 'admin',
      Author: 'admin',
      Influencer: {
        Description: {
          Text: selectedInfluencer.description
        },
        Photo: {
          Paths: selectedInfluencer.photo.paths,
          Urls: selectedInfluencer.photo.urls
        },
        Fanpage: {
          Text: selectedInfluencer.fanpage
        },
        Email: {
          Text: ''
        },
        FullName: {
          Text: selectedInfluencer.fullName
        },
        ShareLink: {
          Text: selectedInfluencer.shareLink
        },
        PostImage: {
          Text: selectedInfluencer.postImage
        },
        LiveStream: {
          Text: selectedInfluencer.liveStream
        },
        CheckIn: {
          Text: selectedInfluencer.checkIn
        },
        Video: {
          Text: selectedInfluencer.video
        },
        Phone: {
          Text: selectedInfluencer.phone
        },
        NumberOfShare: {
          Text: selectedInfluencer.numberOfShare
        },
        NumberOfReaction: {
          Text: selectedInfluencer.numberOfReaction
        },
        NumberOfComment: {
          Text: selectedInfluencer.numberOfComment
        }
      },
      TitlePart: {
        Title: selectedInfluencer.displayText
      },
      AgeDemorgraphic: {
        AgePercentage: {
          Text: selectedInfluencer.agePercentage
        },
        AgeGraphicsName: {
          Text: selectedInfluencer.ageGraphicsName
        }
      },
      GenderDemorgraphic: {
        GenderPercentage: {
          Text: selectedInfluencer.genderDemorgraphic
        },
        GenderGraphicName: {
          Text: selectedInfluencer.genderGraphicName
        }
      },
      GeoDemorgraphic: {
        GeoPercentage: {
          Text: selectedInfluencer.geoPercentage
        },
        GeoGraphicName: {
          Text: selectedInfluencer.geoGraphicName
        }
      }
    }

    campaignContentItems.push(influencerLocal);
  }

  const compaignType = {
    ContentItemId: '',
    ContentItemVersionId: '',
    ContentType: 'Campaign',
    DisplayText: campaign.campaignName + ';' + brandName,
    Latest: true,
    Published: false,
    ModifiedUtc: '',
    PublishedUtc: '',
    CreatedUtc: '',
    Owner: 'admin',
    Author: 'admin',
    Campaign: {
      CampaignTarget: {
        Text: campaign.campaignTarget
      },
      FromDate: {
        Value: fromDate
      },
      ToDate: {
        Value: toDate
      },
      ProductInfo: {
        Text: campaign.productInfo
      },
      Budget: {
        Value: campaign.budget
      },
      Currency: {
        Text: 'VND'
      },
      CampaignName: {
        Text: campaign.campaignName
      },
      MarketPlace: {
        Text: locationString
      },
      FromAge: {
        Value: campaign.fromAge
      },
      ToAge: {
        Value: campaign.toAge
      },
      Gender: {
        Value: campaign.gender
      },
      JobName: {
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
      },
      Interesting: {
        Text: interestingString
      }
    },
    TitlePart: {
      Title: campaign.campaignName + ';' + brandName,
    },
    BagPart: {
      ContentItems: campaignContentItems
    },
    CheckIn: {
      Cost: {
        Text: isCheckIn ? selectedInfluencer.checkIn : 0
      }
    },
    LiveStream: {
      Cost: {
        Text: isLiveStream ? selectedInfluencer.liveStream : 0
      }
    },
    PostImage: {
      Cost: {
        Text: isPostImage ? selectedInfluencer.postImage : 0
      }
    },
    ShareLink: {
      Cost: {
        Text: isShareLink ? selectedInfluencer.shareLink : 0
      }
    },
    Video: {
      Cost: {
        Text: isVideo ? videoCost : 0
      }
    }
  }

  //var myJSON = JSON.stringify(compaignType);

  return compaignType;
}