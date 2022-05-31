import React, { useState, useContext, memo, useEffect } from 'react';
import { connect } from 'react-redux';
import { Row, Col, Card, Button, Space, Checkbox, Radio, Select, Popover } from 'antd';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import {
	faCartArrowDown,
	faSortAmountDown,
	faSortAmountUp,
	faTh,
	faThLarge,
	faThList
} from '@fortawesome/free-solid-svg-icons';
import { getAppMode, setGridPreference } from '@damtoken';
import { LowFrequencyContext } from '@damcontext';
import { DefaultFilterState, FilterStateKeys, OPTIONS } from '../../constants';
import { useTranslation } from 'react-i18next';
import ContentHeader, { DropDownOptions } from './contentHeader';

function AssetsHeader(props) {
	const { setGridState, filterState, setFilterState, selectedCollection } = props;

	const { approvalFlag, allCountries, allAccounts } = useContext(LowFrequencyContext);
	const [accountsOptions, setAccountsOptions] = useState([]);
	const [countriesOptions, setCountriesOptions] = useState([]);

	const SortByOptions = approvalFlag
		? [
				OPTIONS.SORTBY.DisplayName.label,
				OPTIONS.SORTBY.DateCreated.label,
				OPTIONS.SORTBY.DateApproved.label,
				OPTIONS.SORTBY.DateRejected.label,
				OPTIONS.SORTBY.DateSubmitted.label
		  ]
		: [OPTIONS.SORTBY.DisplayName.label, OPTIONS.SORTBY.DateCreated.label];

	const assetTypesOptions = [
		OPTIONS.FILTERBY.AssetType.Photos,
		OPTIONS.FILTERBY.AssetType.Videos,
		OPTIONS.FILTERBY.AssetType.Audios,
		OPTIONS.FILTERBY.AssetType.Document,
		OPTIONS.FILTERBY.AssetType.Others
	];

	const statusOptions = [
		OPTIONS.FILTERBY.Status.Draft,
		OPTIONS.FILTERBY.Status.SubmittedForReview,
		OPTIONS.FILTERBY.Status.Approved,
		OPTIONS.FILTERBY.Status.Rejected
	];

	const tagTypesOptions = [OPTIONS.FILTERBY.Tag.AI_Tag, OPTIONS.FILTERBY.Tag.User_Tag];

	const FilterKeys = FilterStateKeys();

	useEffect(() => {
		if (allAccounts) {
			const accountsOptions = allAccounts.map((account) => {
				return { label: account.name, value: account.id };
			});
			setAccountsOptions(accountsOptions);
		}
		if (allCountries) {
			const countriesOptions = allCountries.map((country) => {
				return { label: country.name, value: country.id };
			});
			setCountriesOptions(countriesOptions);
		}
	}, [allAccounts, allCountries]);

	const changeAssets = (type, slug) => {
		setFilterState({ ...filterState, [type]: slug });
	};

	const setSortOrderResult = (slug) => {
		setFilterState({ ...filterState, SortOrder: slug });
	};

	function onChangeView(values) {
		setGridState(values);
		setGridPreference(values);
	}

	const { t } = useTranslation();

	return (
		<ContentHeader
			title={t('Slider.Assets')}
			extraButtons={
				<>
					<Col>
						<DropDownOptions
							title={t('Button.Filter By')}
							subtitle={t('Filter By.Filter Options')}
							onClear={() => {
								setFilterState(
									Object.assign(
										{},
										filterState,
										(({ Status, Type, Tag, Account, Country }) => ({ Status, Type, Tag, Account, Country }))(
											DefaultFilterState
										)
									)
								);
							}}
							content={
								<>
									<Col>
										{approvalFlag && (
											<Row type="flex" justify="space-between" align="top">
												<Col span={24} className="filter-options-title" style={{ fontWeight: 700 }} align="left">
													{t('ModalDetail.Status')}
												</Col>
												<Col span={24} align="left" className="filter-checkbox">
													<Checkbox.Group
														value={filterState.Status}
														onChange={(slug) => changeAssets(FilterKeys.Status, slug)}
														style={{ display: 'flex', flexDirection: 'column' }}
													>
														{statusOptions.map((item) => {
															return (
																<Checkbox
																	style={{ marginLeft: 0 }}
																	value={item.value}
																	className={`ant-checkbox-group-item
																			${
																				item.value === 'Draft'
																					? 'checkbox-draft'
																					: item.value === 'Approved'
																					? 'checkbox-approved'
																					: item.value === 'Rejected'
																					? 'checkbox-rejected'
																					: ''
																			}`}
																>
																	{t(`Status.${item.value}`)}
																</Checkbox>
															);
														})}
													</Checkbox.Group>
												</Col>
											</Row>
										)}

										<Row type="flex" align="top" style={{ marginTop: 10 }}>
											<Col span={24} className="filter-options-title" style={{ fontWeight: 700 }} align="left">
												{t('File Types')}
											</Col>
											<Col span={24} align="left">
												<Checkbox.Group
													style={{ display: 'flex', flexDirection: 'column' }}
													options={assetTypesOptions}
													value={filterState.Type}
													onChange={(slug) => changeAssets(FilterKeys.Type, slug)}
												/>
											</Col>
										</Row>

										<Row type="flex" align="top" style={{ marginTop: 10 }}>
											<Col span={24} className="filter-options-title" style={{ fontWeight: 700 }} align="left">
												{t('Tag Types')}
											</Col>
											<Col span={24} align="left">
												<Checkbox.Group
													style={{ display: 'flex', flexDirection: 'column' }}
													options={tagTypesOptions}
													value={filterState.Tag}
													onChange={(slug) => changeAssets(FilterKeys.Tag, slug)}
												/>
											</Col>
										</Row>
									</Col>
									<Col>
										<Row type="flex" align="top">
											<Col span={24} className="filter-options-title" style={{ fontWeight: 700 }} align="left">
												{t('ModalDetail.Account')}
											</Col>
											<Col span={24} align="left">
												<Checkbox.Group
													style={{ display: 'flex', flexDirection: 'column' }}
													options={accountsOptions}
													value={filterState.Account}
													onChange={(slug) => changeAssets(FilterKeys.Account, slug)}
												/>
											</Col>
										</Row>
										<Row type="flex" align="top" style={{ marginTop: 10 }}>
											<Col span={24} className="filter-options-title" style={{ fontWeight: 700 }} align="left">
												{t('Country')}
											</Col>
											<Col span={24} align="left">
												<Checkbox.Group
													style={{ display: 'flex', flexDirection: 'column' }}
													options={countriesOptions}
													value={filterState.Country}
													onChange={(slug) => changeAssets(FilterKeys.Country, slug)}
												/>
											</Col>
										</Row>
									</Col>
								</>
							}
						/>
						<DropDownOptions
							title={t('Button.Sort By')}
							subtitle={t('Sort By.Sort Options')}
							onClear={() => {
								setFilterState(
									Object.assign(
										{},
										filterState,
										(({ SortBy, SortOrder }) => ({
											SortBy,
											SortOrder
										}))(DefaultFilterState)
									)
								);
							}}
							content={
								<>
									<Row style={{ display: 'flex', flexWrap: 'nowrap', alignItems: 'center' }}>
										<Col>
											<Radio.Group
												style={{ display: 'flex', flexDirection: 'column' }}
												value={filterState.SortBy}
												onChange={(e) => {
													changeAssets(FilterKeys.SortBy, e.target.value);
												}}
											>
												{SortByOptions.map((option) => {
													return (
														<Radio value={option} key={option.key}>
															{option}
														</Radio>
													);
												})}
											</Radio.Group>
										</Col>
										<Col>
											<Button
												type="link"
												className="primary"
												onClick={() => {
													setSortOrderResult(!filterState.SortOrder);
												}}
												icon={<FontAwesomeIcon icon={filterState.SortOrder ? faSortAmountUp : faSortAmountDown} />}
											/>
										</Col>
									</Row>
								</>
							}
						/>
						<DropDownOptions
							title={
								<FontAwesomeIcon
									icon={
										getAppMode().CardGridPreference === 1
											? faThLarge
											: getAppMode().CardGridPreference === 2
											? faTh
											: faThList
									}
									size="lg"
								/>
							}
							content={
								<>
									<Radio.Group
										defaultValue={getAppMode().CardGridPreference}
										onChange={(e) => onChangeView(e.target.value)}
										className="custom-radio-group"
										size="large"
									>
										<Radio.Button value={1}>
											<FontAwesomeIcon icon={faThLarge} size="lg" />
										</Radio.Button>
										<Radio.Button value={2}>
											<FontAwesomeIcon icon={faTh} size="lg" />
										</Radio.Button>
										<Radio.Button value={3}>
											<FontAwesomeIcon icon={faThList} size="lg" />
										</Radio.Button>
									</Radio.Group>
								</>
							}
						/>
					</Col>
					{selectedCollection && (
						<Space>
							{selectedCollection.name}
							<FontAwesomeIcon size="2x" icon={faCartArrowDown} />
						</Space>
					)}
				</>
			}
		/>
	);
}

function mapStateToProps(state) {
	return {};
}

function mapDispatchToProps(dispatch) {
	return {};
}

export default connect(mapStateToProps, mapDispatchToProps)(memo(AssetsHeader));
