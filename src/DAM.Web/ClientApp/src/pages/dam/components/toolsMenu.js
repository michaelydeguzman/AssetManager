import { LowFrequencyContext } from '@damcontext';
import { getUserRole } from '@damtoken';
import {
	faArrowRight,
	faCheckCircle,
	faDownload,
	faFile,
	faFileArchive,
	faFolder,
	faPen,
	faPlus,
	faShareAlt,
	faTrashAlt,
	faCopy,
	faClipboardList
} from '@fortawesome/free-solid-svg-icons';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { Button, Checkbox, Col, Dropdown, Menu, Row, Select, Space, Tag, Upload } from 'antd';
import React, { memo, useContext, useEffect, useState } from 'react';
import { useTranslation } from 'react-i18next';
import { connect } from 'react-redux';
import DownloadModal from './modals/downloadModal';
import { FaAngleDown } from 'react-icons/fa';
import ImportToProjectsModal from './modals/importToProjectsModal';

function ToolsMenu(props) {
	const { t } = useTranslation();
	const {
		modal,
		checkedAssets,
		isDynamicsAddFromDam,
		clickUploaderProps,
		checkedAssetsItem,
		setCheckedAssetsItem,
		setCheckedAssets,
		setShareModalState,
		dataSource,
		keySearchUsed,
		modalDispatch,
		treeDispatch,
		touchFolder,
		modalState,
		approvalFlag,
		setSelectedAssetID,
		selectAll,
		setSelectAll,
		setSelectedShareAssets,
		setShareFolderModalState,
		filterState,
		setFilterState,
		hasWatermark,
		selectedCollection
	} = props;
	const userRole = getUserRole();

	const [checkList, setCheckList] = useState([]);
	const { allCountries, allAccounts } = useContext(LowFrequencyContext);
	const [accountsOptions, setAccountsOptions] = useState([]);
	const [countriesOptions, setCountriesOptions] = useState([]);
	const [isDownloadModalOpen, setIsDownloadModalOpen] = useState(false);
	const [isImportToProjectsModalOpen, setIsImportToProjectsModalOpen] = useState(false);
	const [downloadExt, setDownloadExt] = useState('');

	useEffect(() => {
		setFilterState({ ...filterState, Search: keySearchUsed !== [] ? [keySearchUsed, []] : [[], []] });
	}, [keySearchUsed]);
	useEffect(() => {
		var newCheckList = [];
		Object.keys(filterState).map((type) => {
			if (typeof filterState[type] === 'string' && filterState[type] !== '') {
				newCheckList.push({ [type]: filterState[type] });
			} else if (typeof filterState[type] === 'object') {
				filterState[type].map((item) => {
					if (typeof item === 'number' || (typeof item === 'string' && item !== '')) {
						newCheckList.push({ [type]: item });
					} else if (typeof item[0] === 'string') {
						item.map((delta) => newCheckList.push({ [type]: delta }));
					}
				});
			}
		});
		setCheckList(newCheckList);
	}, [filterState]);
	useEffect(() => {
		if (allAccounts) {
			let tempOptions = {};
			allAccounts.map((account) => {
				tempOptions[account.id] = account.name;
			});
			setAccountsOptions(tempOptions);
		}
		if (allCountries) {
			let tempOptions = {};
			allCountries.map((country) => {
				tempOptions[country.id] = country.name;
			});
			setCountriesOptions(tempOptions);
		}
	}, [allAccounts, allCountries]);

	function handleSelectAll() {
		if (selectAll) {
			setCheckedAssets([]);
			setCheckedAssetsItem([]);
			setSelectedShareAssets([]);
		} else {
			let tempItems = dataSource();

			if (tempItems && tempItems.length > 0) {
				let tempIds = tempItems.map((d) => {
					return d.id;
				});
				setCheckedAssets(tempIds);
				setCheckedAssetsItem(tempItems);
			}
		}
		setSelectAll(!selectAll);
	}

	function getResult(key, value, isAsc) {
		switch (key) {
			case 'Account':
				return accountsOptions[value];
			case 'Country':
				return countriesOptions[value];
			case 'SortBy':
				return `${isAsc ? 'Asc' : 'Desc'}: ${value}`;
			default:
				return value;
		}
	}

	function handleDownloadClick(ext) {
		var filterList = checkedAssetsItem.filter(
			(x) => x.fileType.includes('image') || (x.extension.includes('pdf') && ext.length > 0)
		);

		if (hasWatermark && filterList.length > 0) {
			setIsDownloadModalOpen(true);
			setDownloadExt(ext);
		} else {
			downloadAssets(checkedAssetsItem, ext);
		}
	}

	function handleImportToProjectsClick() {
		setIsImportToProjectsModalOpen(true);
	}

	function downloadAssets(checkedAssetsItem, ext) {
		modal().download(checkedAssetsItem, false, ext);
	}

	function filterTagRender(list) {
		const key = Object.keys(list.value)[0];
		const value = Object.values(list.value)[0];
		const onClose = (e) => {
			if (key === 'Search') {
				var slug = filterState[key];
				var index = slug[0].indexOf(value);
				if (index > -1) {
					slug[0].splice(index, 1);
				}
			} else if (key === 'SortBy') {
				slug = '';
			} else {
				var slug = filterState[key];
				var index = slug.indexOf(value);
				if (index > -1) {
					slug.splice(index, 1);
				}
			}
			setCheckedAssets([]);
			setCheckedAssetsItem([]);
			setSelectedShareAssets([]);
			setFilterState({ ...filterState, [key]: slug });
		};
		return (
			<Tag
				key={value}
				className={`pill ${key === 'Search' ? 'color-pill-alt' : 'color-pill-primary'}`}
				visible
				closable
				onClose={onClose}
				style={{ marginRight: 3 }}
			>
				{getResult(key, value, filterState.SortOrder)}
			</Tag>
		);
	}

	const downloadMenu = () => {
		return (
			<Menu>
				<Menu.Item onClick={handleDownloadClick}>Original</Menu.Item>

				{checkedAssetsItem[0].extension.includes('pdf') && (
					//(checkedAssetsItem[0].fileType.includes('image') && !checkedAssetsItem[0].extension.includes('png')))
					<Menu.Item onClick={() => handleDownloadClick('png')}>PNG</Menu.Item>
				)}

				{checkedAssetsItem[0].extension.includes('pdf') && (
					//(checkedAssetsItem[0].fileType.includes('image') && !checkedAssetsItem[0].extension.includes('jpg') && !checkedAssetsItem[0].extension.includes('jpeg')))
					<Menu.Item onClick={() => handleDownloadClick('jpeg')}>JPEG</Menu.Item>
				)}

				{(checkedAssetsItem[0].fileType.includes('image') ||
					checkedAssetsItem[0].extension.includes('doc') ||
					checkedAssetsItem[0].extension.includes('ppt') ||
					checkedAssetsItem[0].extension.includes('xls')) && (
					<Menu.Item onClick={() => handleDownloadClick('pdf')}>PDF</Menu.Item>
				)}
			</Menu>
		);
	};

	return (
		<>
			<Row style={{ display: 'flex', justifyContent: 'space-between' }}>
				<Col id="asset-count">
					<Space>
						{checkedAssets.length > 0 ? (
							<Tag
								closable
								visible
								onClose={() => {
									setCheckedAssets([]);
									setCheckedAssetsItem([]);
									setSelectedShareAssets([]);
									setSelectAll(false);
								}}
								style={{ marginRight: '-8px' }}
							>
								{t('ToolMenu.Selected', { number: `${checkedAssets.length}/${dataSource().length}` })}
							</Tag>
						) : (
							<Tag style={{ marginRight: '-8px' }}>{t('ToolMenu.Total Assets', { number: dataSource().length })}</Tag>
						)}
						{dataSource().length > 0 ? (
							<Checkbox
								style={{ marginLeft: '8px' }}
								className="rounded-checkbox"
								checked={selectAll}
								onChange={handleSelectAll}
							>
								{t('ToolMenu.Select All')}
							</Checkbox>
						) : (
							''
						)}
					</Space>
				</Col>
				<Col className="toolbox-upload-btn">
					{!isDynamicsAddFromDam && userRole.canUpload && (
						<Upload {...clickUploaderProps}>
							<Button type="primary" style={{ width: '100%' }}>
								{t('Button.Upload')}
							</Button>
						</Upload>
					)}
				</Col>
			</Row>
			{checkList.length > 0 ? (
				<Row className="filter-tags">
					<Select
						mode="multiple"
						allowClear
						tagRender={filterTagRender}
						open={false}
						value={checkList}
						onClear={() => {
							setCheckedAssets([]);
							setCheckedAssetsItem([]);
							setSelectedShareAssets([]);
							setFilterState({});
						}}
					/>
				</Row>
			) : (
				<></>
			)}
			<Row style={{ marginTop: 16 }}>
				<Col>
					{checkedAssets.length > 0 ? (
						<Space className="toolMenu">
							{userRole.canEdit && checkedAssets.length === 1 && (
								<Col>
									<Button
										className="option-button"
										onClick={modal().rename}
										icon={
											<span className="fa-layers fa-fw">
												<FontAwesomeIcon icon={faFile} size="lg" />
												<FontAwesomeIcon icon={faPen} color="white" transform="shrink-6.4 down-2" />
											</span>
										}
									>
										{t('Button.Rename')}
									</Button>
								</Col>
							)}

							{checkedAssets.length > 0 && selectedCollection && (
								<Col>
									<Button
										className="option-button"
										onClick={modal().addToCart}
										icon={
											<span className="fa-layers fa-fw" style={{ marginRight: '8px' }}>
												<FontAwesomeIcon icon={faFolder} size="lg" />
												<FontAwesomeIcon icon={faPlus} color="white" transform="shrink-8.4 left-4" />
											</span>
										}
									>
										{t('Button.AddCollection')}
									</Button>
								</Col>
							)}

							{checkedAssets.length > 0 && (
								<Col>
									<Button
										className="option-button"
										onClick={handleImportToProjectsClick}
										icon={
											<span className="fa-layers fa-fw">
												<FontAwesomeIcon icon={faClipboardList} size="lg" />
												<FontAwesomeIcon icon={faPlus} color="white" transform="shrink-8.4 left-4" />
											</span>
										}
									>
										{t('Button.ImportToProject')}
									</Button>
								</Col>
							)}

							{userRole.canArchive && (
								<Col>
									<Button
										className="option-button"
										onClick={modal().archive}
										icon={<FontAwesomeIcon icon={faFileArchive} size="lg" />}
									>
										{t('Button.Archive')}
									</Button>
								</Col>
							)}
							{userRole.canMove && (
								<Col>
									<Button
										className="option-button"
										onClick={modal().move}
										icon={
											<span className="fa-layers fa-fw">
												<FontAwesomeIcon icon={faFile} size="lg" />
												<FontAwesomeIcon icon={faArrowRight} color="white" transform="shrink-6.4 down-2" />
											</span>
										}
									>
										{t('Button.Move')}
									</Button>
								</Col>
							)}
							<Col>
								{checkedAssets.length === 1 &&
								(checkedAssetsItem[0].fileType.includes('image') ||
									checkedAssetsItem[0].extension.includes('pdf') ||
									checkedAssetsItem[0].extension.includes('doc') ||
									checkedAssetsItem[0].extension.includes('xls') ||
									checkedAssetsItem[0].extension.includes('ppt')) ? (
									<Dropdown overlay={downloadMenu} placement="bottomRight">
										<Button
											className="option-button"
											icon={
												<span className="fa-layers fa-fw">
													<FontAwesomeIcon icon={faFile} size="lg" />
													<FontAwesomeIcon icon={faDownload} color="white" transform="shrink-6.4 down-2" />
												</span>
											}
										>
											<Space>
												{t('Button.Download')}
												<FaAngleDown className="icons" onClick={(e) => e.preventDefault()} style={{ marginTop: 5 }} />
											</Space>
										</Button>
									</Dropdown>
								) : (
									<Button
										className="option-button"
										onClick={handleDownloadClick}
										icon={
											<span className="fa-layers fa-fw">
												<FontAwesomeIcon icon={faFile} size="lg" />
												<FontAwesomeIcon icon={faDownload} color="white" transform="shrink-6.4 down-2" />
											</span>
										}
									>
										{t('Button.Download')}
									</Button>
								)}
							</Col>
							{userRole.canShare && (
								<Col>
									<Button
										className="option-button"
										onClick={() => {
											setSelectedAssetID(checkedAssetsItem[0].id);
											setSelectedShareAssets(checkedAssetsItem);
											setShareModalState(true);
										}}
										icon={
											<span className="fa-layers fa-fw">
												<FontAwesomeIcon icon={faFile} size="lg" />
												<FontAwesomeIcon icon={faShareAlt} color="white" transform="shrink-6.4 down-2" />
											</span>
										}
									>
										{t('Button.Share')}
									</Button>
								</Col>
							)}
							{checkedAssets.length >= 1 && (
								<>
									{userRole.canApprove && approvalFlag && (
										<Col>
											<Button
												className="option-button"
												onClick={() => modal().approval(checkedAssetsItem[0])}
												icon={
													<span className="fa-layers">
														<FontAwesomeIcon icon={faFile} size="lg" />
														<FontAwesomeIcon icon={faCheckCircle} color="white" transform="shrink-5 down-4 right-2" />
													</span>
												}
											>
												{t('Button.Send for Approval')}
											</Button>
										</Col>
									)}
								</>
							)}
						</Space>
					) : (
						!isDynamicsAddFromDam && (
							<Space size={[8, 16]} wrap="true">
								{modalState.type === 'edit-only' ? (
									<Col>
										<Button
											className="link-button"
											onClick={() => {
												modalDispatch({});
												treeDispatch({
													type: 'CHECKED_KEYS',
													payload: []
												});
											}}
										>
											{t('Button.Cancel')}
										</Button>
									</Col>
								) : (
									<Col>
										<Button
											className="option-button"
											disabled={modalState.type === 'edit-only'}
											hidden={true}
											onClick={modal().edit}
											icon={<FontAwesomeIcon icon={faPen} size="lg" />}
										>
											{t('Button.Edit')}
										</Button>
									</Col>
								)}
								{userRole.canAdd && (
									<Col>
										<Button
											className="option-button"
											onClick={modal().addFolder}
											icon={
												<span className="fa-layers fa-fw">
													<FontAwesomeIcon icon={faFolder} size="lg" />
													<FontAwesomeIcon icon={faPlus} color="white" transform="shrink-8.4 left-4" />
												</span>
											}
										>
											{/* {isDesktopView && "Add"} */}
											{t('Button.Add')}
										</Button>
									</Col>
								)}

								{keySearchUsed.length === 0 && touchFolder > 2 && touchFolder !== null && userRole.canMove && (
									<Col>
										<Button
											className="option-button"
											onClick={modal().copyFolder}
											icon={
												<span className="fa-layers fa-fw">
													<FontAwesomeIcon icon={faFolder} size="lg" />
													<FontAwesomeIcon icon={faCopy} color="white" transform="shrink-8.4 left-4" />
												</span>
											}
										>
											Copy
										</Button>
									</Col>
								)}

								{!dataSource().length &&
									keySearchUsed.length === 0 &&
									touchFolder > 1 &&
									touchFolder !== null &&
									userRole.canDelete && (
										<Col>
											<Button
												className="option-button"
												onClick={modal().dropFolder}
												icon={
													<span className="fa-layers fa-fw">
														<FontAwesomeIcon icon={faFolder} size="lg" />
														<FontAwesomeIcon icon={faTrashAlt} color="white" transform="shrink-8.4 left-4" />
													</span>
												}
											>
												{t('Button.Delete')}
											</Button>
										</Col>
									)}
								{keySearchUsed.length === 0 && touchFolder > 2 && touchFolder !== null && userRole.canMove && (
									<Col>
										<Button
											className="option-button"
											onClick={modal().moveFolder}
											icon={
												<span className="fa-layers fa-fw">
													<FontAwesomeIcon icon={faFolder} size="lg" />
													<FontAwesomeIcon icon={faArrowRight} color="white" transform="shrink-8.4 left-4" />
												</span>
											}
										>
											{t('Button.Move')}
										</Button>
									</Col>
								)}

								{userRole.canEdit && (
									<Col>
										<Button
											className="option-button"
											onClick={modal().editMetadata}
											icon={
												<span className="fa-layers fa-fw">
													<FontAwesomeIcon icon={faFolder} size="lg" />
													<FontAwesomeIcon icon={faPen} color="#fff" transform="shrink-8.4 left-4" />
												</span>
											}
										>
											{t('Button.Edit')}
										</Button>
									</Col>
								)}
								{userRole.canShare && (
									<Col>
										<Button
											className="option-button"
											onClick={() => {
												setShareFolderModalState(true);
											}}
											icon={
												<span className="fa-layers fa-fw">
													<FontAwesomeIcon icon={faFile} size="lg" />
													<FontAwesomeIcon icon={faShareAlt} color="white" transform="shrink-6.4 down-2" />
												</span>
											}
										>
											{t('Button.Share')}
										</Button>
									</Col>
								)}
							</Space>
						)
					)}
				</Col>
			</Row>

			<DownloadModal
				isDownloadModalOpen={isDownloadModalOpen}
				setIsDownloadModalOpen={setIsDownloadModalOpen}
				modal={modal}
				checkedAssetsItem={checkedAssetsItem}
				downloadExt={downloadExt}
			/>

			<ImportToProjectsModal
				isImportToProjectsModalOpen={isImportToProjectsModalOpen}
				setIsImportToProjectsModalOpen={setIsImportToProjectsModalOpen}
				checkedAssetsItem={checkedAssetsItem}
			/>
		</>
	);
}

function mapStateToProps(state) {
	return {};
}

function mapDispatchToProps(dispatch) {
	return {};
}

export default connect(mapStateToProps, mapDispatchToProps)(memo(ToolsMenu));
