import React, { Component } from "react";
import { Link } from 'react-router-dom';
import { connect } from 'react-redux';
//import img from '../assets/images/hand.jpg'
//import { campaignActions } from '../_actions';
import { campaignActions, infActions, brandActions } from '../../../_actions';
import Select from 'react-select';
import { createLocations, createInterestings, createJobs } from '../../../_models/CommonModels';
import city from '../../../assets/utils/images/originals/city.jpg'
import citynights from '../../../assets/utils/images/originals/citynights.jpg'
import citydark from '../../../assets/utils/images/originals/citydark.jpg'
import defaultAvatar from '../../../assets/utils/images/avatars/default.jpg'
import DateRangePicker from 'react-daterange-picker'
import originalMoment from "moment";
import { extendMoment } from "moment-range";
import NumberFormat from 'react-number-format';
import 'react-daterange-picker/dist/css/react-calendar.css'
import JwPagination from 'jw-react-pagination';
//import { SearchBox } from '../SearchBox';
import cx from 'classnames';
import Skeleton from 'react-loading-skeleton';
import new_logo from '../../../assets/utils/images/new_logo.png'
import {
    Row, Col, CustomInput
} from 'reactstrap';

import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';

import {
    faAngleUp
} from '@fortawesome/free-solid-svg-icons';
//import {configContent} from 'configContent';
import { history } from '../../../_helpers';

class RegisterCampaignPage extends Component {

    constructor(props) {
        super(props);

        const moment = extendMoment(originalMoment);

        const today = moment();
        //const startDate = today;
        //const dateValue = today;
        // an example array of items to be paged
        var exampleItems = [...Array(45).keys()].map(i => ({ id: (i + 1), name: 'Item ' + (i + 1) }));

        this.state = {
            campaign: {
                campaignTarget: '',
                marketPlace: 'Toàn quốc',
                fromAge: 19,
                toAge: 30,
                gender: 'Mọi giới',
                campaignName: '',
                fromDate: '',
                toDate: '',
                productInfo: '',
                budget: '',
                currency: 'VND'
            },
            job: {
                jobName: '',
                jobHashTag: '',
                jobKeyword: '',
                jobDescription: '',
                jobLink: '',
                jobTasks: []
            },
            selectedOptionLocation: [{ value: "TPHCM", label: "TPHCM" }],
            selectedOptionInteresting: [{ value: "Fashion", label: "Fashion" }],
            selectedOptionJobCategory: [{ value: 'Share Link', label: 'Share Link' }, { value: 'Post Image', label: 'Post Image' }],
            selectedInfluencers: [],
            submitted: false,
            isFormStep: true,
            isInfluencerStep: false,
            isJobStep: false,
            isChecked: false,
            skip: 0,
            checkedInfluencers: new Map(),
            startDate: today,
            endDate: today,
            isOpen: false,
            pageOfItems: [],
            exampleItems,
            first: 9,
            searchValue: '',
            dateValue: moment.range(today.clone(), today.clone().add(7, "days"))
        };

        this.handleCampaignChange = this.handleCampaignChange.bind(this);
        this.handleJobChange = this.handleJobChange.bind(this);
        this.handleSubmitJobs = this.handleSubmitJobs.bind(this);
        this.handleOptionLocationChange = this.handleOptionLocationChange.bind(this);
        this.handleOptionInterestingChange = this.handleOptionInterestingChange.bind(this);
        this.handleOptionJobCategoryChange = this.handleOptionJobCategoryChange.bind(this);
        this.handleCheckBoxChange = this.handleCheckBoxChange.bind(this);
        this.handleInfluencerStep = this.handleInfluencerStep.bind(this);
        this.handleBackStep = this.handleBackStep.bind(this);
        this.handleJobStep = this.handleJobStep.bind(this);
        this.nextPageFluencers = this.nextPageFluencers.bind(this);
        this.prePageFluencers = this.prePageFluencers.bind(this);
        this.onChangePage = this.onChangePage.bind(this);
        this.handleSearch = this.handleSearch.bind(this);
    }

    nextPageFluencers(skip) {
        this.setState({ skip: skip + 1 });
    }

    prePageFluencers() {
        const { skip } = this.state;
        if (skip > 0) {
            this.setState({ skip: skip - 1 });
        }
    }

    handleCampaignChange(event) {
        const { name, value } = event.target;
        const { campaign } = this.state;
        this.setState({
            campaign: {
                ...campaign,
                [name]: value
            }
        });
    }

    handleJobChange(event) {
        const { name, value } = event.target;
        const { job } = this.state;
        this.setState({
            job: {
                ...job,
                [name]: value
            }
        });
    }

    handleSubmitJobs(event) {

        event.preventDefault();

        this.setState({ submitted: true });
        const { campaign,
            job,
            selectedOptionLocation,
            selectedOptionInteresting,
            selectedOptionJobCategory,
            selectedInfluencers,
            checkedInfluencers,
            dateValue } = this.state;
        const { dispatch, brands } = this.props;
        const { brand, userName } = this.props.location.state;
        //const brandObject = brand? brand.Brand: brands.brand;
        let brandObject = {};

        if (brand) {
            brandObject = {
                brandName: brand.Brand.BrandName.Text,
                fullName: brand.Brand.FullName.Text,
                businessAreas: brand.Brand.BusinessAreas.Text,
                location: brand.Brand.Location.Text,
            }
        }
        else {
            brandObject = {
                brandName: brands.brand.brandName,
                fullName: brands.brand.fullName,
                businessAreas: brands.brand.businessAreas,
                location: brands.brand.location,
            }
        }

        if (campaign.campaignName &&
            campaign.campaignTarget &&
            dateValue.start &&
            dateValue.end &&
            campaign.fromAge &&
            campaign.toAge &&
            campaign.productInfo &&
            campaign.gender &&
            campaign.budget &&
            selectedOptionLocation &&
            selectedOptionInteresting &&
            selectedOptionJobCategory &&
            job.jobDescription &&
            job.jobName &&
            selectedInfluencers.length > 0) {

            this.setState({ isFormStep: true, isInfluencerStep: false, isJobStep: false });

            dispatch(campaignActions.register(campaign,
                dateValue.start.format("DD MMM YYYY"),
                dateValue.end.format("DD MMM YYYY"),
                job,
                selectedOptionLocation,
                selectedOptionInteresting,
                selectedOptionJobCategory,
                brandObject.brandName,
                brandObject.fullName,
                brandObject.businessAreas,
                brandObject.location,
                selectedInfluencers,
                checkedInfluencers));
        }
    }

    handleInfluencerStep(event) {
        const { campaign, dateValue, selectedOptionLocation, selectedOptionInteresting } = this.state;
        event.preventDefault();

        this.setState({ submitted: true });
        if (campaign.campaignName &&
            campaign.campaignTarget &&
            dateValue.start &&
            dateValue.end &&
            campaign.fromAge &&
            campaign.toAge &&
            campaign.productInfo &&
            campaign.gender &&
            campaign.budget &&
            selectedOptionLocation &&
            selectedOptionInteresting) {

            this.setState({ isFormStep: false, isInfluencerStep: true, isJobStep: false });
        }
    }

    handleJobStep(event) {
        event.preventDefault();
        //this.setState({ submitted: true });
        const { campaign,
            job,
            dateValue,
            selectedOptionLocation,
            selectedOptionInteresting,
            selectedInfluencers,
            checkedInfluencers } = this.state;
        const { dispatch, brands } = this.props;

        if (campaign.campaignName &&
            campaign.campaignTarget &&
            dateValue.start &&
            dateValue.end &&
            campaign.fromAge &&
            campaign.toAge &&
            campaign.productInfo &&
            campaign.gender &&
            campaign.budget &&
            selectedOptionLocation &&
            selectedOptionInteresting &&
            checkedInfluencers.size > 0) {

            this.setState({ isFormStep: false, isInfluencerStep: false, isJobStep: true });
        }
    }

    handleBackStep(event) {
        event.preventDefault();
        const { isInfluencerStep, isJobStep } = this.state;
        if (isInfluencerStep == true) {
            this.setState({ isFormStep: true, isInfluencerStep: false, isJobStep: false });
            return;
        }

        if (isJobStep == true) {
            this.setState({ isFormStep: false, isInfluencerStep: true, isJobStep: false });
            return;
        }
        //dispatch(infActions.getAll());
    }

    handleOptionLocationChange = selectedOptionLocation => {
        this.setState({ selectedOptionLocation });
        //console.log(`Option selected:`, selectedOptionLocation);
    };

    handleOptionInterestingChange = selectedOptionInteresting => {
        this.setState({ selectedOptionInteresting });
        //console.log(`Option selected:`, selectedOptionInteresting);
    };

    handleOptionJobCategoryChange = selectedOptionJobCategory => {
        this.setState({ selectedOptionJobCategory });
        //console.log(`Option selected:`, selectedOptionJobCategory);
    };

    handleCheckBoxChange(event) {

        const { influencers } = this.props;
        const { selectedInfluencers } = this.state;
        const selectedInfluencersLocal = selectedInfluencers;

        const item = event.target.name;
        const isChecked = event.target.checked;

        this.setState(prevState => ({ checkedInfluencers: prevState.checkedInfluencers.set(item, isChecked) }));

        influencers.items.influencer.map((item, key) => {
            if (item.contentItemId === event.target.name && isChecked == true) {
                selectedInfluencersLocal.push(item);
            }
            else if (item.contentItemId === event.target.name && isChecked == false) {
                selectedInfluencersLocal.splice(selectedInfluencersLocal.indexOf(item), 1);
            }
        });

        this.setState({ selectedInfluencers: selectedInfluencersLocal })
    };

    componentDidMount() {
        const { dispatch } = this.props;
        const { first } = this.state;

        if (this.props.location.state) {
            const { brand, userName } = this.props.location.state;

            if (brand) {
                //const { brand } = this.props.location.state;
                dispatch(infActions.getAll(first, 0));
                //dispatch(brandActions.getBrandFromBrandPage(brand));
            }
            else {
                if (userName) {
                    dispatch(brandActions.getBrandByName(userName));
                }
                else {
                    history.push('/registerBrandPage');
                }
            }
        }
        else {
            history.push('/registerBrandPage');
        }

    }

    onDatesChange = (dateValue) => {
        //
        const { campaign } = this.state;

        if (dateValue) {
            this.setState({ isOpen: false, dateValue: dateValue });
        }
    }

    onToggle = () => {
        const { isOpen } = this.state;
        this.setState({ isOpen: !isOpen });
    };

    handleSearch(searchValue) {
        const { dispatch } = this.props;
        const { first } = this.state;
        this.setState({ searchValue: searchValue });
        if (searchValue !== '') {
            dispatch(infActions.getInfluencersByName(first, 0, searchValue));
        } else {
            dispatch(infActions.getAll(first, 0));
        }
    }

    onChangePage(pageOfItems) {
        const { dispatch } = this.props;
        const { first, searchValue } = this.state;
        const length = pageOfItems.length;
        const currentPage = Math.ceil(pageOfItems[length - 1].id / first);

        dispatch(infActions.getInfluencersByName(first, first * (currentPage - 1), searchValue));
        // update local state with new page of items
        this.setState({ pageOfItems });
    }

    render() {
        const { submitted,
            campaign,
            job,
            selectedOptionLocation,
            selectedOptionInteresting,
            selectedOptionJobCategory,
            isFormStep,
            isInfluencerStep,
            isJobStep,
            isOpen,
            dateValue,
            exampleItems,
            first } = this.state;
        const { influencers, campaigns } = this.props;
        const infItems = influencers.items ? influencers.items.influencer : [];
        let imgSrc = defaultAvatar;

        const locations = createLocations();
        const interestings = createInterestings();
        const jobs = createJobs();
        const colors = [
            "bg-mean-fruit",
            "bg-tempting-azure",
            "bg-amy-crisp",
            "bg-arielle-smile",
            "bg-happy-itmeo",
            "bg-strong-bliss",
            "bg-mean-fruit",
            "bg-tempting-azure",
            "bg-amy-crisp",
        ];

        const { brand, userName } = this.props.location.state;

        const brandFromLoading = this.props.brands.brand;

        return (
            <div className="app-container app-theme-white body-tabs-shadow">
                <div className="app-container">
                    <div className="h-100">
                        {
                            isFormStep &&
                            <div className="h-100 no-gutters row">
                                <div className="h-100 d-md-flex d-sm-block bg-white justify-content-center align-items-center col-md-12 col-lg-8">
                                    <div className="mx-auto app-login-box col-sm-12 col-md-10 col-lg-9">
                                        <div style={{ marginBottom: '3rem', width: '97px', height: '23px' }}>
                                            <img src={new_logo} alt="Kols Viet" />
                                        </div>
                                        <h4>
                                            <div>Welcome,</div>
                                            <span>It only takes a <span className="text-success">few minutes</span> to create your Campaign</span></h4>
                                        <div>
                                            <form className="" onSubmit={this.handleInfluencerStep} id="register-form">
                                                <div className="form-row">
                                                    <div className="col-md-6">
                                                        <div className="position-relative form-group">
                                                            <label htmlFor="name" className="">
                                                                <span className="text-danger">*</span> Campaign Name</label>
                                                            <input type="text" name="campaignName" id="campaignName" placeholder="Campaign Name" value={campaign.campaignName} onChange={this.handleCampaignChange} className="form-control" />
                                                            {
                                                                submitted && !campaign.campaignName &&
                                                                <div className="help-block text-danger">Campaign Name is required</div>
                                                            }
                                                        </div>
                                                    </div>
                                                    <div className="col-md-6">
                                                        <div className="position-relative form-group">
                                                            <label htmlFor="pass" className="">
                                                                <span className="text-danger">*</span> Campaign Target</label>
                                                            <input type="text" className="form-control" name="campaignTarget" id="campaignTarget" placeholder="Campaign Target" value={campaign.campaignTarget} onChange={this.handleCampaignChange} />
                                                            {
                                                                submitted && !campaign.campaignTarget &&
                                                                <div className="help-block text-danger">Campaign Target is required</div>
                                                            }
                                                        </div>
                                                    </div>
                                                    <div className="col-md-6">
                                                        <div className="position-relative form-group">
                                                            <label htmlFor="name" className="">
                                                                <span className="text-danger">*</span> Product Info
                                                            </label>
                                                            <input type="text" className="form-control" name="productInfo" id="productInfo" placeholder="Product Info" value={campaign.productInfo} onChange={this.handleCampaignChange} />
                                                            {
                                                                submitted && !campaign.productInfo &&
                                                                <div className="help-block text-danger">Product Info is required</div>
                                                            }
                                                        </div>
                                                    </div>
                                                    <div className="col-md-6">
                                                        <div className="position-relative form-group">
                                                            <label htmlFor="name" className=""> <span className="text-danger">*</span> Date Time</label>
                                                            <div>
                                                                <input className="form-control" name="dateRange"
                                                                    type="text"
                                                                    value={'From ' + dateValue.start.format("DD-MM-YYYY") + ' To ' + dateValue.end.format("DD-MM-YYYY")}
                                                                    onChange={this.onDatesChange}
                                                                    onClick={this.onToggle} />
                                                            </div>

                                                            {isOpen && (
                                                                <DateRangePicker value={dateValue} onSelect={this.onDatesChange} singleDateRange={true} />
                                                            )}
                                                            {
                                                                (submitted && !dateValue) &&
                                                                <div className="help-block text-danger">Campaign Date is required</div>
                                                            }
                                                        </div>
                                                    </div>
                                                    <div className="col-md-3">
                                                        <div className="position-relative form-group">
                                                            <label htmlFor="name" className="">
                                                                <span className="text-danger">*</span> From Age
                                                        </label>
                                                            <input type="number" className="form-control" name="fromAge" id="fromAge" placeholder="From Age" value={campaign.fromAge} onChange={this.handleCampaignChange} />
                                                            {
                                                                submitted && !campaign.fromAge &&
                                                                <div className="help-block text-danger">Age is required</div>
                                                            }
                                                        </div>
                                                    </div>
                                                    <div className="col-md-3">
                                                        <div className="position-relative form-group">
                                                            <label htmlFor="name" className="">
                                                                <span className="text-danger">*</span> To Age
                                                        </label>
                                                            <input type="number" className="form-control" name="toAge" id="toAge" placeholder="To Age" value={campaign.toAge} onChange={this.handleCampaignChange} />
                                                            {
                                                                submitted && !campaign.toAge &&
                                                                <div className="help-block text-danger">Age is required</div>
                                                            }
                                                        </div>
                                                    </div>
                                                    <div className="col-md-6">
                                                        <div className="position-relative form-group">
                                                            <label htmlFor="pass" className="">
                                                                <span className="text-danger">*</span> Location</label>
                                                            <Select
                                                                value={selectedOptionLocation}
                                                                onChange={this.handleOptionLocationChange}
                                                                isMulti
                                                                placeholder="Locations..."
                                                                options={locations}
                                                            />
                                                        </div>
                                                    </div>
                                                    <div className="col-md-4">
                                                        <div className="position-relative form-group">
                                                            <label htmlFor="pass" className="">
                                                                <span className="text-danger">*</span> Interestings</label>
                                                            <Select
                                                                value={selectedOptionInteresting}
                                                                onChange={this.handleOptionInterestingChange}
                                                                isMulti
                                                                placeholder="Interestings..."
                                                                options={interestings}
                                                            />
                                                        </div>
                                                    </div>
                                                    <div className="col-md-4">
                                                        <div className="position-relative form-group">
                                                            <label htmlFor="pass" className="">
                                                                <span className="text-danger">*</span> Gender</label>
                                                            <input type="text" className="form-control" name="gender" id="gender" placeholder="Gender" value={campaign.gender} onChange={this.handleCampaignChange} />
                                                            {
                                                                submitted && !campaign.gender &&
                                                                <div className="help-block text-dander">Gender is required</div>
                                                            }
                                                        </div>
                                                    </div>
                                                    <div className="col-md-4">
                                                        <div className="position-relative form-group">
                                                            <label htmlFor="pass" className="">
                                                                <span className="text-danger">*</span> Budget</label>
                                                            <NumberFormat className="form-control" name="budget" id="budget" thousandSeparator={true} suffix={'đ'} value={campaign.budget} placeholder="Price..." onValueChange={(values) => {
                                                                const { value } = values;
                                                                const { campaign } = this.state;
                                                                const campaignLocal = campaign;
                                                                campaignLocal.budget = value;
                                                                this.setState({ campaign: campaignLocal })
                                                            }} />
                                                            {/* <input type="text" className="form-control" name="budget" id="budget" placeholder="Budget" value={campaign.budget} onChange={this.handleCampaignChange} /> */}
                                                            {
                                                                submitted && !campaign.budget &&
                                                                <div className="help-block text-danger">Budget is required</div>
                                                            }
                                                        </div>
                                                    </div>
                                                </div>
                                                <div className="mt-4 d-flex align-items-center">
                                                    <div className="ml-auto">
                                                        <input type="submit" name="nextStep" id="nextStep" className="btn-wide btn-pill btn-shadow btn-hover-shine btn btn-primary btn-lg" value="Next Step" />
                                                        {/* <Link to="/pages/loginpage" className="btn btn-link">Cancel</Link> */}
                                                    </div>
                                                </div>
                                            </form>
                                        </div>
                                    </div>
                                </div>
                                <div className="d-lg-flex d-xs-none col-lg-4">
                                    <div className="slider-light">
                                        <div className="slick-slider slick-initialized">
                                            <div>
                                                <div className="position-relative h-100 d-flex justify-content-center align-items-center bg-premium-dark" tabIndex="-1">
                                                    <div className="slide-img-bg" style={{ backgroundImage: `url(${citynights})` }}></div>
                                                    <div className="slider-content"><h3>Scalable, Modular, Consistent</h3>
                                                        <p>Easily exclude the components you don't require. Lightweight, consistent Bootstrap based styles across all elements and components</p></div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        }
                        {
                            //isInfluencerStep &&
                        //     <div className="h-100 no-gutters row">
                        //         <div className="h-200 d-md-flex d-sm-block bg-white justify-content-center align-items-center col-md-12 col-lg-12">
                        //             <div className="mx-auto app-login-box col-sm-12 col-md-10 col-lg-12">
                        //                 <div className="app-header bg-strong-bliss header-text-light header-shadow">
                        //                     <div className={cx(
                        //                         "app-header__content",
                        //                     )}>
                        //                         <div className="app-header-left">
                        //                             <SearchBox handlerFromParent={this.handleSearch} />
                        //                         </div>
                        //                     </div>
                        //                 </div>
                        //                 <ReactCSSTransitionGroup
                        //                     component="div"
                        //                     transitionName="TabsAnimation"
                        //                     transitionAppear={true}
                        //                     transitionAppearTimeout={0}
                        //                     transitionEnter={false}
                        //                     transitionLeave={false}>
                        //                     {
                        //                         influencers.loading ?
                        //                             <Skeleton count={30} /> :
                        //                             <Row>
                        //                                 {
                        //                                     influencers.items && influencers.items.influencer.map((item, key) => {
                        //                                         if ((brand && brand.published) || (brandFromLoading && brandFromLoading.published)) {
                        //                                             imgSrc = item.photo.urls.length == 0 ? defaultAvatar : "http://bdo8.com" + item.photo.urls[0] + '?&width=240&height=240&rmode=';
                        //                                         }
                        //                                         return (
                        //                                             <Col key={key} md="4">
                        //                                                 <div className={"card mb-3 widget-chart " + colors[key] + " card-border"}>
                        //                                                     <div className="rounded-circle">
                        //                                                         <img className="mx-auto rounded-circle" style={{ width: '88px', height: '88px' }} src={imgSrc} alt="" />
                        //                                                     </div>
                        //                                                     <div className="divide" style={{ marginBottom: '5px' }} />
                        //                                                     <div className="widget-heading">
                        //                                                         {item.fullName} - {item.description}
                        //                                                     </div>
                        //                                                     <Row>
                        //                                                         <Col>
                        //                                                             <div className="widget-subheading" style={{ textAlign: 'left' }}>
                        //                                                                 Hướng về lứa: {item.ageDemorgraphic.ageGraphicsName}
                        //                                                             </div>
                        //                                                         </Col>
                        //                                                         <Col>
                        //                                                             <div className="widget-subheading" style={{ textAlign: 'left' }}>
                        //                                                                 {
                        //                                                                     ((brand && brand.published) || (brandFromLoading && brandFromLoading.published)) ?
                        //                                                                         'Share Link: ' + (item.shareLink ? item.shareLink : 'Call') : 'Share Link: 1000000'
                        //                                                                 }
                        //                                                             </div>
                        //                                                         </Col>
                        //                                                     </Row>
                        //                                                     <Row>
                        //                                                         <Col>
                        //                                                             <div className="widget-subheading" style={{ textAlign: 'left' }}>
                        //                                                                 Hướng về giới: {item.genderDemorgraphic.genderGraphicsName}
                        //                                                             </div>
                        //                                                         </Col>
                        //                                                         <Col>
                        //                                                             <div className="widget-subheading" style={{ textAlign: 'left' }}>
                        //                                                                 {
                        //                                                                     ((brand && brand.published) || (brandFromLoading && brandFromLoading.published)) ?
                        //                                                                         'Post Image: ' + (item.postImage ? item.postImage : 'Call') : 'Post Image: 1000000'
                        //                                                                 }
                        //                                                             </div>
                        //                                                         </Col>
                        //                                                     </Row>
                        //                                                     <Row>
                        //                                                         <Col>
                        //                                                             <div className="widget-subheading" style={{ textAlign: 'left' }}>
                        //                                                                 Hướng về nơi: {item.geoDemorgraphic.geoGraphicName}
                        //                                                             </div>
                        //                                                         </Col>
                        //                                                         <Col>
                        //                                                             <div className="widget-subheading" style={{ textAlign: 'left' }}>
                        //                                                                 {
                        //                                                                     ((brand && brand.published) || (brandFromLoading && brandFromLoading.published)) ?
                        //                                                                         'Video: ' + (item.video ? item.video : 'Call') : 'Video: 1000000'
                        //                                                                 }
                        //                                                             </div>
                        //                                                         </Col>
                        //                                                     </Row>
                        //                                                     <Row>
                        //                                                         <Col>
                        //                                                         </Col>
                        //                                                         <Col>
                        //                                                             <div className="widget-subheading" style={{ textAlign: 'left' }}>
                        //                                                                 {
                        //                                                                     ((brand && brand.published) || (brandFromLoading && brandFromLoading.published)) ?
                        //                                                                         'CheckIn: ' + (item.checkIn ? item.checkIn : 'Call') : 'CheckIn: 1000000'
                        //                                                                 }
                        //                                                             </div>
                        //                                                         </Col>
                        //                                                     </Row>
                        //                                                     <Row>
                        //                                                         <Col>
                        //                                                         </Col>
                        //                                                         <Col>
                        //                                                             <div className="widget-subheading" style={{ textAlign: 'left' }}>
                        //                                                                 {
                        //                                                                     ((brand && brand.published) || (brandFromLoading && brandFromLoading.published)) ?
                        //                                                                         'LiveStream: ' + (item.liveStream ? item.liveStream : 'Call') : 'LiveStream: 1000000'
                        //                                                                 }
                        //                                                             </div>
                        //                                                         </Col>
                        //                                                     </Row>
                        //                                                     <div className="widget-description text-success">
                        //                                                         <CustomInput type="checkbox" id={item.contentItemId} name={item.contentItemId} onChange={this.handleCheckBoxChange} checked={this.state.checkedInfluencers.get(item.contentItemId) ? this.state.checkedInfluencers.get(item.contentItemId) : false}
                        //                                                             label="Select" />
                        //                                                     </div>
                        //                                                 </div>
                        //                                             </Col>
                        //                                         )
                        //                                     })
                        //                                 }
                        //                             </Row>
                        //                     }
                        //                 </ReactCSSTransitionGroup>
                        //                 <div className="app-header header-text-light header-shadow">
                        //                     <div className={cx(
                        //                         "app-header__content",
                        //                     )}>
                        //                         <div className="app-header-right">
                        //                             <JwPagination disableDefaultStyles={true} pageSize={first} items={exampleItems} onChangePage={this.onChangePage} />
                        //                         </div>
                        //                     </div>
                        //                 </div>
                        //                 <div className="mt-4 d-flex align-items-center">
                        //                     <div className="ml-auto">
                        //                         <input type="button" onClick={this.handleBackStep} name="backinf" id="backinf" className="btn-wide btn-pill btn-shadow btn-hover-shine btn btn-primary btn-lg" value="Back" />
                        //                         <input type="button" onClick={this.handleJobStep} name="job" id="job" className="btn-wide btn-pill btn-shadow btn-hover-shine btn btn-primary btn-lg" value="Last Step" />
                        //                     </div>
                        //                 </div>
                        //             </div>
                        //         </div>
                        //     </div>
                        // }
                        //{
                            isJobStep &&
                            <div className="h-100 no-gutters row">
                                <div className="h-100 d-md-flex d-sm-block bg-white justify-content-center align-items-center col-md-12 col-lg-8">
                                    <div className="mx-auto app-login-box col-sm-12 col-md-10 col-lg-9">
                                        <div>
                                            <form className="" onSubmit={this.handleSubmitJobs} id="register-form">
                                                <div className="form-row">
                                                    <div className="col-md-5">
                                                        <div className="position-relative form-group">
                                                            <label htmlFor="jobName" className="">
                                                                <span className="text-danger">*</span> Job Name</label>
                                                            <input type="text" name="jobName" id="jobName" placeholder="Name of your job" value={job.jobName} onChange={this.handleJobChange} className="form-control" />
                                                            {
                                                                submitted && !job.jobName &&
                                                                <div className="help-block text-danger">Job Name is required</div>
                                                            }
                                                        </div>
                                                    </div>
                                                    <div className="col-md-7">
                                                        <div className="position-relative form-group">
                                                            <label htmlFor="name" className="">
                                                                <span className="text-danger">*</span> Job Description</label>
                                                            <input type="text" className="form-control" name="jobDescription" id="jobDescription" placeholder="Description of your job" value={campaign.jobDescription} onChange={this.handleJobChange} />
                                                            {
                                                                submitted && !job.jobDescription &&
                                                                <div className="help-block text-danger">Job Description is required</div>
                                                            }
                                                        </div>
                                                    </div>
                                                    <div className="col-md-6">
                                                        <div className="position-relative form-group">
                                                            <label htmlFor="jobHashTag" className="">
                                                                <span></span> Job HashTag</label>
                                                            <input type="text" className="form-control" name="jobHashTag" id="jobHashTag" placeholder="Ex: #hashtag1;#hashtag2" value={job.jobHashTag} onChange={this.handleJobChange} />
                                                        </div>
                                                    </div>
                                                    <div className="col-md-6">
                                                        <div className="position-relative form-group">
                                                            <label htmlFor="jobKeyword" className="">
                                                                <span></span> Job Keyword</label>
                                                            <input type="text" className="form-control" name="jobKeyword" id="jobKeyword" placeholder="Ex: nice" value={job.jobKeyword} onChange={this.handleJobChange} />
                                                        </div>
                                                    </div>
                                                    <div className="col-md-12">
                                                        <div className="position-relative form-group">
                                                            <label htmlFor="jobLink" className="">
                                                                <span></span> Job Link</label>
                                                            <input type="text" className="form-control" name="jobLink" id="jobLink" placeholder="Link of your page" value={job.jobLink} onChange={this.handleJobChange} />
                                                        </div>
                                                    </div>
                                                    <div className="col-md-7">
                                                        <div className="position-relative form-group">
                                                            <label htmlFor="jobCategory" className="">
                                                                <span className="text-danger">*</span> Job</label>
                                                            <Select
                                                                value={selectedOptionJobCategory}
                                                                onChange={this.handleOptionJobCategoryChange}
                                                                isMulti
                                                                options={jobs}
                                                            />
                                                        </div>
                                                    </div>
                                                </div>
                                                <div className="mt-4 d-flex align-items-center">
                                                    <div className="ml-auto">
                                                        <input type="button" onClick={this.handleBackStep} name="backinf" id="backinf" className="btn-wide btn-pill btn-shadow btn-hover-shine btn btn-primary btn-lg" value="Back" />
                                                        <input type="submit" name="register" id="register" className="btn-wide btn-pill btn-shadow btn-hover-shine btn btn-primary btn-lg" value="Register" />
                                                        {
                                                            campaigns.loading &&
                                                            <img src="data:image/gif;base64,R0lGODlhEAAQAPIAAP///wAAAMLCwkJCQgAAAGJiYoKCgpKSkiH/C05FVFNDQVBFMi4wAwEAAAAh/hpDcmVhdGVkIHdpdGggYWpheGxvYWQuaW5mbwAh+QQJCgAAACwAAAAAEAAQAAADMwi63P4wyklrE2MIOggZnAdOmGYJRbExwroUmcG2LmDEwnHQLVsYOd2mBzkYDAdKa+dIAAAh+QQJCgAAACwAAAAAEAAQAAADNAi63P5OjCEgG4QMu7DmikRxQlFUYDEZIGBMRVsaqHwctXXf7WEYB4Ag1xjihkMZsiUkKhIAIfkECQoAAAAsAAAAABAAEAAAAzYIujIjK8pByJDMlFYvBoVjHA70GU7xSUJhmKtwHPAKzLO9HMaoKwJZ7Rf8AYPDDzKpZBqfvwQAIfkECQoAAAAsAAAAABAAEAAAAzMIumIlK8oyhpHsnFZfhYumCYUhDAQxRIdhHBGqRoKw0R8DYlJd8z0fMDgsGo/IpHI5TAAAIfkECQoAAAAsAAAAABAAEAAAAzIIunInK0rnZBTwGPNMgQwmdsNgXGJUlIWEuR5oWUIpz8pAEAMe6TwfwyYsGo/IpFKSAAAh+QQJCgAAACwAAAAAEAAQAAADMwi6IMKQORfjdOe82p4wGccc4CEuQradylesojEMBgsUc2G7sDX3lQGBMLAJibufbSlKAAAh+QQJCgAAACwAAAAAEAAQAAADMgi63P7wCRHZnFVdmgHu2nFwlWCI3WGc3TSWhUFGxTAUkGCbtgENBMJAEJsxgMLWzpEAACH5BAkKAAAALAAAAAAQABAAAAMyCLrc/jDKSatlQtScKdceCAjDII7HcQ4EMTCpyrCuUBjCYRgHVtqlAiB1YhiCnlsRkAAAOwAAAAAAAAAAAA==" />
                                                        }
                                                    </div>
                                                </div>
                                            </form>
                                        </div>
                                    </div>
                                </div>
                                <div className="d-lg-flex d-xs-none col-lg-4">
                                    <div className="slider-light">
                                        <div className="slick-slider slick-initialized">
                                            <div>
                                                <div className="position-relative h-100 d-flex justify-content-center align-items-center bg-premium-dark" tabIndex="-1">
                                                    <div className="slide-img-bg" style={{ backgroundImage: `url(${citynights})` }}></div>
                                                    <div className="slider-content"><h3>Scalable, Modular, Consistent</h3>
                                                        <p>Easily exclude the components you don't require. Lightweight, consistent Bootstrap based styles across all elements and components</p></div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        }
                    </div>
                </div>
            </div>
        );
    }
}

function mapStateToProps(state) {
    
    const { campaigns, influencers, locations, interestings, jobCategories, jobs, brands } = state;
    //const { brand } = influencers;
    return {
        //loggingIn,
        brands,
        jobs,
        jobCategories,
        interestings,
        locations,
        campaigns,
        influencers
    };
}

const connectedRegisterCampaignPage = connect(mapStateToProps)(RegisterCampaignPage);
export { connectedRegisterCampaignPage as RegisterCampaignPage };