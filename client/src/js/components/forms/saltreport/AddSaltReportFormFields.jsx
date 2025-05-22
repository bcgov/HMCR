import React, { useEffect, useState } from 'react';
import { connect } from 'react-redux';
import { debounce } from 'lodash';
import _ from 'lodash';
import { Field, FieldArray, useFormikContext } from 'formik';
import { validationSchema } from './ValidationSchema';
import { defaultValues, tooltips } from './DefaultValues';
import PageSpinner from '../../ui/PageSpinner';
import SingleDropdownField from '../../ui/SingleDropdownField';
import { TooltipProvider, useTooltip } from '../../../contexts/TooltipContext';
import { FormRow, FormInput, FormCheckboxInput, FormRadioInput, FormNumberInput } from '../FormInputs';
import { Row, Col, FormGroup, Table, Button, Tooltip } from 'reactstrap';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';

const materialStorageAppendixLabel = {
  materialStorage: {
    newSaltDomeWithPad: 'Install a new salt dome(s) with an impervious pad',
    newSaltDomeIndoorStorage:
      'Install a new salt dome(s) with indoor storage for all salts and treated abrasives and inside loading capacity',
    upgradeSaltStorageSites: 'Upgrade existing salt storage sites to add a permanent roof',
    constructPermanentCoverStructure:
      'Construct a permanent structure to cover salts (can be made of different rigid materials including wood, steel, aluminum, fibreglass, or fabric)',
    impermeablePadForAbrasives: 'Install an impermeable pad to store treated abrasive',
    expandInsideBuildingForAbrasives: 'Construct or expand inside building capacity to cover treated abrasives',
    useTarpsForAbrasives: 'Use tarps to cover abrasives',
    reconfigureStorageCapacity: 'Reconfigure storage capacity to store abrasives within an existing storage structure',
    reconfigureOperationFacilities: 'Reconfigure operation facilities to conduct equipment washing inside',
    designAreaForTruckLoading: 'Design an area specifically for truck loading',
    controlTruckLoading: 'Establish a method to control truck loading',
    installEquipmentWashBay:
      'Install equipment wash bay (including collection and treatment of wastewater with oil/grit separator)',
    designSiteForRunoffControl:
      'Design the site to control runoff water and keep it away from working areas and material storage. Ensure water from outside is diverted from the storage sites.',
    manageSaltContaminatedWaters:
      'Design a collection system and manage salt contaminated waters. Select option for disposal: transported from the site for treatment, brine production, released to municipal storm water or release to the environment',
    spillPreventionPlan:
      'Ensure measures and a plan are in place to prevent or reduce the probability or the significance of a spill (e.g. recover mechanism for de-icing liquid tanks in case of leaking, regular inspection of installation and recording)',
    removeContaminatedSnow:
      'Remove and dispose of salt contaminated snow from the storage site in a snow disposal site',
    otherSpecify: 'Other (specify):',
  },
  saltApplication: {
    installGroundSpeedControls: 'Install ground speed oriented electronic spreader controls',
    increasePreWettingEquipment: 'Increase the number of trucks with pre-wetting equipment (new or retro-fit)',
    installLiquidAntiIcing: 'Install new or replacement of liquid anti-icing equipment',
    installInfraredThermometers: 'Install infrared thermometers on vehicles',
    installAdditionalRWISStations: 'Install additional fixed RWIS station(s)',
    accessRWISData: 'Acquire access to RWIS data provided by others',
    installMobileRWIS: 'Install mobile RWIS to be mounted on vehicle',
    accessMeteorologicalService: 'Acquire access to meteorological service',
    adoptPreWettingMajorityNetwork: 'Adopt pre-wetting practice on the vast majority of the road network',
    usePreTreatedSalt: 'Use pre-treated salt on the vast majority of the road network',
    adoptPreWettingOrTreatmentAbrasives: 'Adopt pre-wetting or pre-treatment of abrasives',
    testingNewProducts:
      'Testing of new products (e.g. organics, de-icer mixed to increase performance at different temperature)',
    adoptAntiIcingStandard: 'Adopt anti-icing as a standard practice (early treatment)',
    installGPSAndComputerSystems:
      'Install GPS and computer system on trucks to record salt application rates & service mapping (route)',
    useChartForApplicationRates:
      'Use of a chart to make decisions on application rates adapted to the meteorological and pavement conditions and level of service',
    useMDSS:
      'Use of a maintenance decision support system that incorporates meteorological data, pavement temperatures and recommended application rates to respond to conditions and plan operations',
    reviewSaltUse:
      'Review yearly overall salt use, identify areas and operations where salt quantities could be reduced (i.e. reduction of the salt applied/road km/year). Ensure level of service for each roadway segment is adequate (check traffic to avoid over use of salt).',
    assessPlowingEfficiency: 'Assess the efficiency of plowing operations (timing, type of plows and blades, etc.)',
    other: 'Other (specify):',
  },
  snowDisposal: {
    developProgramPhaseOut: 'Develop a program to phase out unacceptable snow disposal sites',
    installNewSiteLowPermeability:
      'Install a new snow disposal site(s), ensuring it is entirely on a low permeability surface',
    upgradeExistingSiteLowPermeability:
      'Upgrade an existing snow disposal site(s) and install a low permeability surface',
    collectMeltWaterSpecificPoint: 'Collect all melt water and discharge at a specific point',
    constructCollectionPond:
      'Construct a collection pond to allow water to settle before its discharged, and control the time and rat of discharge',
    otherSnowDisposal: 'Other (specify):',
  },
  vulnerableAreas: {
    identifySaltVulnerableAreas: 'Identify salt vulnerable areas',
    prioritizeAreasForAdditionalProtection:
      'Prioritize areas where additional protection or mitigation measures will be implemented to eliminate or reduce road salt impacts',
    implementProtectionMitigationMeasures:
      'Implement additional protection or mitigation measures to eliminate or reduce road salt impacts on vulnerable areas',
    conductEnvironmentalMonitoring:
      'Conduct environmental monitoring to measure success of mitigation measures for protecting vulnerable areas',
    otherVulnerableAreas: 'Other (specify):',
  },
};

const housekeepingPracticesLabel = {
  allMaterialsHandled: 'All materials are handled in a designated area characterized by an impermeable surface',
  equipmentPreventsOverloading: 'Equipment to prevent overloading of trucks',
  wastewaterSystem: 'System for collection and/or treatment of wastewater from cleaning of trucks',
  controlDiversionExternalWaters: 'Control and diversion of external waters (non salt impacted',
  drainageCollectionSystem: 'Drainage inside with collection systems for runoff of salt contaminated waters',
  municipalSewerSystem: 'Specify discharge point into a municipal sewer system',
  removalContainment: 'Specify discharge point into a containment for removal',
  watercourse: 'Specify discharge point into a watercourse',
  otherDischargePoint: 'Specify discharge point into (other)',
  ongoingCleanup: 'Ongoing cleanup of the site surfaces, and spilled material is swept up quickly',
  riskManagementPlan: 'Risk management and emergency measures plans are in place',
};

const AddSaltReportFormFields = ({ setInitialValues, formValues, setValidationSchema, currentUser }) => {
  const [loading, setLoading] = useState(true);
  const { errors, submitCount, isSubmitting } = useFormikContext();

  const debouncedSaveForm = debounce((values) => {
    sessionStorage.setItem('formData', JSON.stringify(values));
  }, 3000);

  useEffect(() => {
    setLoading(false);
    const defaultValidationSchema = validationSchema.shape({});
    setValidationSchema(defaultValidationSchema);
    setInitialValues(loadFromSessionStorage());

    return () => {
      debouncedSaveForm.cancel();
    };
  }, [setInitialValues, setValidationSchema]);

  useEffect(() => {
    if (isSubmitting && submitCount > 0 && Object.keys(errors).length > 0) {
      scrollToFirstError(errors);
    }
  }, [isSubmitting, submitCount, errors]);

  useEffect(() => {
    debouncedSaveForm(formValues);
  }, [formValues]);

  const loadFromSessionStorage = () => {
    const savedFormData = sessionStorage.getItem('formData');
    return savedFormData ? JSON.parse(savedFormData) : defaultValues;
  };

  const CustomTooltip = ({ tipId, children }) => {
    const { openTooltip, toggleTooltip } = useTooltip();

    return (
      <>
        <span style={{ paddingLeft: '4px' }} href="#" id={tipId}>
          <FontAwesomeIcon icon="question-circle" className="fa-color-primary ml-1 mr-1" />
        </span>
        <Tooltip
          placement="bottom"
          className="fieldset-tooltip"
          autohide={false}
          isOpen={openTooltip === tipId} // Open only if this tooltip's ID matches the openTooltip state
          target={tipId}
          toggle={() => toggleTooltip(tipId)} // Toggle this tooltip's visibility
        >
          <div>{children}</div>
        </Tooltip>
      </>
    );
  };

  // Takes in a array of errors, and recursively search for the property name to scroll to it
  const scrollToFirstError = (errors, prefix = '') => {
    for (const key in errors) {
      if (errors.hasOwnProperty(key)) {
        const error = errors[key];

        if (typeof error === 'string') {
          const selector = `[name="${prefix + key}"]`;

          const errorField = document.querySelector(selector);
          if (errorField) {
            errorField.scrollIntoView({ behavior: 'smooth', block: 'center' });
            errorField.focus();
            return;
          }
        } else if (typeof error === 'object') {
          scrollToFirstError(error, `${prefix + key}.`);
        }
      }
    }
  };

  if (loading || formValues === null) return <PageSpinner />;

  return (
    <TooltipProvider>
      <Row>
        <Col>
          <FormRow name="serviceArea" label="Service Area">
            <SingleDropdownField
              items={_.orderBy(currentUser.serviceAreas, ['id'])}
              defaultTitle="Select Service Area"
              name="serviceArea"
            />
          </FormRow>
        </Col>
        <Col>
          <FormRow name="contactName" label="Contact Name/Title">
            <FormInput type="text" name="contactName" />
          </FormRow>
        </Col>
      </Row>
      <Row>
        <Col></Col>
        <Col></Col>
      </Row>
      <Row>
        <Col>
          <FormRow name="telephone" label="Telephone">
            <FormInput type="text" name="telephone" />
          </FormRow>
        </Col>
        <Col>
          <FormRow name="email" label="Email">
            <FormInput type="text" name="email" />
          </FormRow>
        </Col>
      </Row>
      <section>
        <h4>Section 1: Salt Management Plan</h4>
        <Row className="my-4">
          <Col sm={1}>1.1</Col>
          <Col sm={7}>
            Has your organization developed and implemented a salt management plan that covers all elements described in
            the Code of Practice?
          </Col>
          <Col>
            <div>
              <FormRadioInput id="planDeveloped.no" name="sect1.planDeveloped" value="no" label="No" />
              <FormRadioInput id="planDeveloped.yes" name="sect1.planDeveloped" value="yes" label="Yes" />
            </div>
          </Col>
        </Row>
        <Row className="my-4">
          <Col sm={1}>1.2</Col>
          <Col sm={7}>In the past year, did your organization conduct a review of its salt management plan?</Col>
          <Col>
            <div>
              <FormRadioInput id="planReviewed.no" name="sect1.planReviewed" value="no" label="No" />
              <FormRadioInput id="planReviewed.yes" name="sect1.planReviewed" value="yes" label="Yes" />
            </div>
          </Col>
        </Row>
        <Row className="my-4">
          <Col sm={1}>1.3</Col>
          <Col sm={7}>In the past year, did your organization update its salt management plan?</Col>
          <Col>
            <div>
              <FormRadioInput id="planUpdated.no" name="sect1.planUpdated" value="no" label="No" />
              <FormRadioInput id="planUpdated.yes" name="sect1.planUpdated" value="yes" label="Yes" />
            </div>
          </Col>
        </Row>
        <Row className="my-4">
          <Col sm={1}>1.4</Col>
          <Col sm={7}>
            In the past year, was a training program offered to personnel involved in winter maintenance operations and
            decision making? Indicate which levels of responsibility were offered training (new or refresher):
          </Col>
        </Row>
        <Row className="my-4 justify-content-center">
          <Col xs={2}>
            <div>
              Manager(s)
              <FormRadioInput id="manager.no" name="sect1.training.manager" value="no" label="No" />
              <FormRadioInput id="manager.yes" name="sect1.training.manager" value="yes" label="Yes" />
            </div>
          </Col>
          <Col xs={2}>
            <div>
              Supervisor(s)
              <FormRadioInput id="supervisor.no" name="sect1.training.supervisor" value="no" label="No" />
              <FormRadioInput id="supervisor.yes" name="sect1.training.supervisor" value="yes" label="Yes" />
            </div>
          </Col>
          <Col xs={2}>
            <div>
              Operator(s)
              <FormRadioInput id="operator.no" name="sect1.training.operator" value="no" label="No" />
              <FormRadioInput id="operator.yes" name="sect1.training.operator" value="yes" label="Yes" />
            </div>
          </Col>
          <Col xs={2}>
            <div>
              Mechanical
              <FormRadioInput id="mechanical.no" name="sect1.training.mechanical" value="no" label="No" />
              <FormRadioInput id="mechanical.yes" name="sect1.training.mechanical" value="yes" label="Yes" />
            </div>
          </Col>
          <Col xs={2}>
            <div>
              Patroller(s)
              <FormRadioInput id="patroller.no" name="sect1.training.patroller" value="no" label="No" />
              <FormRadioInput id="patroller.yes" name="sect1.training.patroller" value="yes" label="Yes" />
            </div>
          </Col>
        </Row>
        <Row className="my-4">
          <Col>
            <Row>
              <Col sm={1}>1.5</Col>
              <Col>
                Indicate the number of objectives
                <CustomTooltip tipId="objectives">{tooltips.objectives}</CustomTooltip>
                that were identified and achieved for this year in your salt management plan within the following areas:
                (refer to Appendix A for a sample list of objectives)
              </Col>
            </Row>
            <Row className="my-2">
              <Col>
                <Table bordered style={{ tableLayout: 'fixed' }}>
                  <thead>
                    <tr>
                      <th rowSpan={2}>Areas for Improvement</th>
                      <th colSpan={2}>Number of Objectives for Winter 2024/2025</th>
                    </tr>
                    <tr>
                      <th># Identified</th>
                      <th># Achieved</th>
                    </tr>
                  </thead>
                  <tbody>
                    <tr>
                      <td>Material Storage Facilities</td>
                      <td>
                        <FormNumberInput type="number" name="sect1.objectives.materialStorage.identified" />
                      </td>
                      <td>
                        <FormGroup>
                          <FormNumberInput type="number" name="sect1.objectives.materialStorage.achieved" />
                        </FormGroup>
                      </td>
                    </tr>
                    <tr>
                      <td>Salt Application</td>
                      <td>
                        <FormGroup>
                          <FormNumberInput type="number" name="sect1.objectives.saltApplication.identified" />
                        </FormGroup>
                      </td>
                      <td>
                        <FormGroup>
                          <FormNumberInput type="number" name="sect1.objectives.saltApplication.achieved" />
                        </FormGroup>
                      </td>
                    </tr>
                  </tbody>
                </Table>
              </Col>
            </Row>
          </Col>
        </Row>
      </section>
      <section>
        <h4>Section 2: Winter Operations Information</h4>
        <Row className="my-4">
          <Col sm={1}>2.1</Col>
          <Col sm={7}>
            What is the total length of road on which any salt is applied in your Service Area (roads with or without
            abrasive)?
          </Col>
          <Col>
            <FormNumberInput
              type="number"
              name="sect2.roadTotalLength"
              placeholder="Km of two-lane equivalent (centre line)"
            />
          </Col>
        </Row>
        <Row className="my-4">
          <Col sm={1}>2.2</Col>
          <Col sm={7}>
            What was the total number of days requiring salt application for winter road maintenance during the winter
            season?
          </Col>
          <Col>
            <FormNumberInput type="number" name="sect2.saltTotalDays" placeholder="days" />
          </Col>
        </Row>
      </section>
      <section>
        <h4>Section 3: Materials Applied</h4>
        <Row className="my-4">
          <Col>
            <Row>
              <Col sm={1}>3.1</Col>
              <Col>
                {/* Needs yearly update */}
                Provide the total quantity of material used for winter road maintenance (including sidewalks) as of May
                31st 2025. (If your organization uses multi-chloride<sup>4</sup> products, see question 3.2)
              </Col>
            </Row>
            <Row className="my-2">
              <Col>
                <Table bordered style={{ tableLayout: 'fixed' }}>
                  <thead>
                    <tr>
                      <th colSpan={2}>Solids</th>
                      <th>Tonnes</th>
                    </tr>
                  </thead>
                  <tbody>
                    <tr>
                      <th rowSpan={4}>De-icers</th>
                      <th>Sodium chloride (NaCl)</th>

                      <td>
                        <FormNumberInput type="number" name="sect3.deicer.nacl" step="0.01" />
                      </td>
                    </tr>
                    <tr>
                      <th>
                        Magnesium chloride (MgCl<sub>2</sub>)
                      </th>
                      <td>
                        <FormNumberInput type="number" name="sect3.deicer.mgcl2" step="0.01" />
                      </td>
                    </tr>
                    <tr>
                      <th>
                        Calcium chloride (CaCl<sub>2</sub>)
                      </th>
                      <td>
                        <FormNumberInput type="number" name="sect3.deicer.cacl2" step="0.01" />
                      </td>
                    </tr>
                    <tr>
                      <th>
                        Acetate
                        <CustomTooltip tipId="acetate">{tooltips.acetate}</CustomTooltip>
                      </th>
                      <td>
                        <FormNumberInput type="number" name="sect3.deicer.acetate" step="0.01" />
                      </td>
                    </tr>
                    <tr>
                      <th rowSpan={4}>
                        Treated Abrasives
                        <br />
                        Specify the types & quantities of solid salts added to stockpile of abrasives (freeze protection
                        & free flowing). *Indicate number of tonnes before mixing*{' '}
                      </th>
                      <th>Sand, stone dust, or aggregates</th>
                      <td>
                        <FormNumberInput type="number" name="sect3.treatedAbrasives.sandStoneDust" step="0.01" />
                      </td>
                    </tr>
                    <tr>
                      <th>Sodium chloride (NaCl)</th>
                      <td>
                        <FormNumberInput type="number" name="sect3.treatedAbrasives.nacl" step="0.01" />
                      </td>
                    </tr>
                    <tr>
                      <th>
                        Magnesium chloride (MgCl<sub>2</sub>)
                      </th>
                      <td>
                        <FormNumberInput type="number" name="sect3.treatedAbrasives.mgcl2" step="0.01" />
                      </td>
                    </tr>
                    <tr>
                      <th>
                        Calcium chloride (CaCl<sub>2</sub>)
                      </th>
                      <td>
                        <FormNumberInput type="number" name="sect3.treatedAbrasives.cacl2" step="0.01" />
                      </td>
                    </tr>
                    <tr>
                      <th rowSpan={5}>
                        Pre-wetting Liquid Concentrated liquid product sprayed (with on-board equipment) to the solid
                        de-icing agent or the abrasive directly as it is spread or discharged from the truck to the
                        pavement
                      </th>
                      <th>Sodium chloride (NaCl)</th>
                      <td>
                        <FormNumberInput type="number" name="sect3.prewetting.nacl" step="0.01" />
                      </td>
                    </tr>
                    <tr>
                      <th>
                        Magnesium chloride (MgCl<sub>2</sub>)
                      </th>
                      <td>
                        <FormNumberInput type="number" name="sect3.prewetting.mgcl2" step="0.01" />
                      </td>
                    </tr>
                    <tr>
                      <th>
                        Calcium chloride (CaCl<sub>2</sub>)
                      </th>
                      <td>
                        <FormNumberInput type="number" name="sect3.prewetting.cacl2" step="0.01" />
                      </td>
                    </tr>
                    <tr>
                      <th>
                        Acetate<sup>2</sup>
                      </th>
                      <td>
                        <FormNumberInput type="number" name="sect3.prewetting.acetate" step="0.01" />
                      </td>
                    </tr>
                    <tr>
                      <th>
                        Non-chloride organic products
                        <CustomTooltip tipId="nonchloride">{tooltips.nonchloride}</CustomTooltip>
                      </th>
                      <td>
                        <FormNumberInput type="number" name="sect3.prewetting.nonchloride" step="0.01" />
                      </td>
                    </tr>
                    <tr>
                      <th rowSpan={5}>
                        Pre-treatment Liquid Concentrated liquid product added to the solid de-icer and the abrasive at
                        the time it is stockpiled at the storage site or added by the supplier before delivery
                      </th>
                      <th>Sodium chloride (NaCl)</th>
                      <td>
                        <FormNumberInput type="number" name="sect3.pretreatment.nacl" step="0.01" />
                      </td>
                    </tr>
                    <tr>
                      <th>
                        Magnesium chloride (MgCl<sub>2</sub>)
                      </th>
                      <td>
                        <FormNumberInput type="number" name="sect3.pretreatment.mgcl2" step="0.01" />
                      </td>
                    </tr>
                    <tr>
                      <th>
                        Calcium chloride (CaCl<sub>2</sub>)
                      </th>
                      <td>
                        <FormNumberInput type="number" name="sect3.pretreatment.cacl2" step="0.01" />
                      </td>
                    </tr>
                    <tr>
                      <th>
                        Acetate<sup>2</sup>
                      </th>
                      <td>
                        <FormNumberInput type="number" name="sect3.pretreatment.acetate" step="0.01" />
                      </td>
                    </tr>
                    <tr>
                      <th>Non-chloride organic products3</th>
                      <td>
                        <FormNumberInput type="number" name="sect3.pretreatment.nonchloride" step="0.01" />
                      </td>
                    </tr>
                    <tr>
                      <th rowSpan={5}>
                        Direct Liquid Application (aka Anti-icing) Concentrated liquid product that is sprayed directly
                        on the pavement surface with a truck or by a sprayer system (e.g. Fixed Automated Spray
                        Technology FAST) before a storm or the formation of frost.
                      </th>
                      <th>Sodium chloride (NaCl)</th>
                      <td>
                        <FormNumberInput type="number" name="sect3.antiicing.nacl" step="0.01" />
                      </td>
                    </tr>
                    <tr>
                      <th>
                        Magnesium chloride (MgCl<sub>2</sub>)
                      </th>
                      <td>
                        <FormNumberInput type="number" name="sect3.antiicing.mgcl2" step="0.01" />
                      </td>
                    </tr>
                    <tr>
                      <th>
                        Calcium chloride (CaCl<sub>2</sub>)
                      </th>
                      <td>
                        <FormNumberInput type="number" name="sect3.antiicing.cacl2" step="0.01" />
                      </td>
                    </tr>
                    <tr>
                      <th>
                        Acetate<sup>2</sup>
                      </th>
                      <td>
                        <FormNumberInput type="number" name="sect3.antiicing.acetate" step="0.01" />
                      </td>
                    </tr>
                    <tr>
                      <th>Non-chloride organic products3</th>
                      <td>
                        <FormNumberInput type="number" name="sect3.antiicing.nonchloride" step="0.01" />
                      </td>
                    </tr>
                  </tbody>
                </Table>
              </Col>
            </Row>
          </Col>
        </Row>
        <Row className="my-4">
          <Col>
            <Row>
              <Col sm={1}>3.2</Col>
              <Col>
                If your organization uses multi-chloride liquids
                <CustomTooltip tipId="liquids">{tooltips.liquids}</CustomTooltip>, specify litres used and concentration
                for each salt in the liquid:
              </Col>
            </Row>
            <Row className="my-2">
              <Col>
                <Table bordered>
                  <thead>
                    <tr>
                      <th>Multi-Chloride</th>
                      <th>Litres</th>
                      <th>NaCl %</th>
                      <th>MgCl2 %</th>
                      <th>CaCl2 %</th>
                    </tr>
                  </thead>
                  <tbody>
                    {/* Multi-chloride A */}
                    <tr>
                      <td>Multi-chloride A</td>
                      <td>
                        <FormNumberInput type="number" name="sect3.multiChlorideA.litres" step="0.01" />
                      </td>
                      <td>
                        <FormNumberInput type="number" name="sect3.multiChlorideA.naclPercentage" step="0.01" />
                      </td>
                      <td>
                        <FormNumberInput type="number" name="sect3.multiChlorideA.mgcl2Percentage" step="0.01" />
                      </td>
                      <td>
                        <FormNumberInput type="number" name="sect3.multiChlorideA.cacl2Percentage" step="0.01" />
                      </td>
                    </tr>
                    {/* Multi-chloride B */}
                    <tr>
                      <td>Multi-chloride A</td>
                      <td>
                        <FormNumberInput type="number" name="sect3.multiChlorideB.litres" step="0.01" />
                      </td>
                      <td>
                        <FormNumberInput type="number" name="sect3.multiChlorideB.naclPercentage" step="0.01" />
                      </td>
                      <td>
                        <FormNumberInput type="number" name="sect3.multiChlorideB.mgcl2Percentage" step="0.01" />
                      </td>
                      <td>
                        <FormNumberInput type="number" name="sect3.multiChlorideB.cacl2Percentage" step="0.01" />
                      </td>
                    </tr>
                  </tbody>
                </Table>
              </Col>
            </Row>
          </Col>
        </Row>
      </section>
      <section>
        <h4>Section 4: Design and Operation at Road Salt Storage Sites</h4>
        <Row className="my-4">
          <Col sm={1}>4.1</Col>
          <Col sm={7}>How many salt storage sites (locations, not stockpiles) are in your Service Area?</Col>
          <Col>
            <FormNumberInput type="number" name="sect4.saltStorageSitesTotal" />
          </Col>
        </Row>
        <Row className="my-4">
          <Col>
            <Row>
              <Col sm={1}>4.2</Col>
              <Col>
                Provide the number of stockpiles that are stored under the following conditions. If your organization
                manages more than one site, provide the information for each site (insert additional rows as needed to table below)
              </Col>
            </Row>
            <Row className="my-2">
              <Col>
                <FieldArray name="sect4.stockpiles">
                  {({ insert, remove }) => (
                    <>
                      <Table bordered style={{ tableLayout: 'fixed' }}>
                        <thead>
                          <tr>
                            <th rowSpan={2}>Site Name</th>
                            <th rowSpan={2}>MoTI Owned?</th>
                            <th colSpan={4}>Road Salts</th>
                            <th colSpan={4}>Treated Abrasives</th>
                          </tr>
                          <tr>
                            <th># stockpiles</th>
                            <th>on impermeable surface</th>
                            <th>under permanent roof</th>
                            <th>under tarp</th>
                            <th># stockpiles</th>
                            <th>on impermeable surface</th>
                            <th>under permanent roof</th>
                            <th>under tarp</th>
                          </tr>
                        </thead>
                        <tbody>
                          {formValues.sect4.stockpiles.map((site, index) => (
                            <tr key={index}>
                              <td>
                                <FormInput name={`sect4.stockpiles.${index}.siteName`} />
                              </td>
                              <td>
                                <Field
                                  name={`sect4.stockpiles.${index}.motiOwned`}
                                  type="checkbox"
                                  className="form-control"
                                />
                              </td>
                              <td>
                                <FormNumberInput
                                  type="number"
                                  name={`sect4.stockpiles.${index}.roadSalts.stockpilesTotal`}
                                />
                              </td>
                              <td>
                                <FormNumberInput
                                  type="number"
                                  name={`sect4.stockpiles.${index}.roadSalts.onImpermeableSurface`}
                                />
                              </td>
                              <td>
                                <FormNumberInput
                                  type="number"
                                  name={`sect4.stockpiles.${index}.roadSalts.underPermanentRoof`}
                                />
                              </td>
                              <td>
                                <FormNumberInput type="number" name={`sect4.stockpiles.${index}.roadSalts.underTarp`} />
                              </td>
                              <td>
                                <FormNumberInput
                                  type="number"
                                  name={`sect4.stockpiles.${index}.treatedAbrasives.stockpilesTotal`}
                                />
                              </td>
                              <td>
                                <FormNumberInput
                                  type="number"
                                  name={`sect4.stockpiles.${index}.treatedAbrasives.onImpermeableSurface`}
                                />
                              </td>
                              <td>
                                <FormNumberInput
                                  type="number"
                                  name={`sect4.stockpiles.${index}.treatedAbrasives.underPermanentRoof`}
                                />
                              </td>
                              <td>
                                <FormNumberInput
                                  type="number"
                                  name={`sect4.stockpiles.${index}.treatedAbrasives.underTarp`}
                                />
                              </td>
                              <td>
                                <Button type="button" onClick={() => remove(index)}>
                                  Remove
                                </Button>
                              </td>
                            </tr>
                          ))}
                        </tbody>
                      </Table>
                      <Button
                        onClick={() =>
                          insert(formValues.sect4.stockpiles.length, {
                            siteName: '',
                            motiOwned: false,
                            roadSalts: {
                              stockpilesTotal: null,
                              onImpermeableSurface: null,
                              underPermanentRoof: null,
                              underTarp: null,
                            },
                            treatedAbrasives: {
                              stockpilesTotal: null,
                              onImpermeableSurface: null,
                              underPermanentRoof: null,
                              underTarp: null,
                            },
                          })
                        }
                      >
                        Add Stockpile
                      </Button>
                    </>
                  )}
                </FieldArray>
              </Col>
            </Row>
          </Col>
        </Row>
        <Row className="my-4">
          <Col>
            <Row>
              <Col sm={1}>4.3</Col>
              <Col>
                Provide the characteristics of your storage site design and the working activities that support good
                housekeeping practices. If your organization manages more than one site, indicate the number of sites
                with the given characteristic.
              </Col>
            </Row>
            <Row className="my-2">
              <Col>
                <Table bordered>
                  <thead>
                    <tr>
                      <th>Good Housekeeping Practice</th>
                      <th>Plan in Place (Y/N)</th>
                      <th>Number of Sites</th>
                    </tr>
                  </thead>
                  <tbody>
                    {Object.entries(formValues.sect4.practices).map(([key, value], index) => (
                      <tr key={index}>
                        <td>{housekeepingPracticesLabel[key]}</td>
                        <td>
                          <Field name={`sect4.practices.${key}.hasPlan`} type="checkbox" className="form-control" />
                        </td>
                        <td>
                          <FormNumberInput
                            type="number"
                            name={`sect4.practices.${key}.numSites`}
                            className="form-control"
                          />
                        </td>
                      </tr>
                    ))}
                  </tbody>
                </Table>
              </Col>
            </Row>
          </Col>
        </Row>
      </section>
      <section>
        <h4>Section 5: Salt Application</h4>
        <Row className="my-4">
          <Col>
            <Row className="my-2">
              <Col sm={1}>5.1</Col>
              <Col>a) Indicate the total number of vehicles (trucks) used for winter maintenance:</Col>
              <Col>
                <FormNumberInput type="number" name="sect5.numberOfVehicles" />
              </Col>
            </Row>
            <Row>
              <Col sm={1} />
              <Col>b) Indicate the number of vehicles used for salt application (with or without plowing).</Col>
            </Row>
            <Row className="my-2">
              <Col sm={1} />
              <Col>
                <Table bordered>
                  <thead>
                    <tr>
                      <th>Vehicle Equipment</th>
                      <th>Number of Vehicles</th>
                    </tr>
                  </thead>
                  <tbody>
                    <tr>
                      <td>Total number of vehicles assigned to solid salt application</td>
                      <td>
                        <FormNumberInput type="number" name="sect5.vehiclesForSaltApplication" />
                      </td>
                    </tr>
                    <tr>
                      <td>Vehicles with conveyors and ground speed sensor electronic controller</td>
                      <td>
                        <FormNumberInput type="number" name="sect5.vehiclesWithConveyors" />
                      </td>
                    </tr>
                    <tr>
                      <td>Vehicles equipped with pre-wetting equipment</td>
                      <td>
                        <FormNumberInput type="number" name="sect5.vehiclesWithPreWettingEquipment" />
                      </td>
                    </tr>
                    <tr>
                      <td>Vehicles designed for direct liquid application (DLA)</td>
                      <td>
                        <FormNumberInput type="number" name="sect5.vehiclesForDLA" />
                      </td>
                    </tr>
                  </tbody>
                </Table>
                <Row>
                  <Col>
                    Spreading equipment is regularly calibrated?
                    <Field name="sect5.regularCalibration" type="checkbox" className="form-control" />
                  </Col>
                  <Col>
                    Frequency
                    <FormNumberInput type="number" name="sect5.regularCalibrationTotal" placeholder="times per year" />
                  </Col>
                </Row>
              </Col>
            </Row>
            <Row></Row>
          </Col>
        </Row>
        <Row className="my-4">
          <Col sm={2}>
            <b>Weather Monitoring</b>
          </Col>
        </Row>
        <Row className="my-4">
          <Col sm={1}>5.2</Col>
          <Col sm={7}>
            Indicate the sources of information your organization relies on to make decisions for winter event
            responses, supplementing road patrol observations.
            <Row className="my-2">
              <Col>
                <Table bordered>
                  <thead>
                    <tr>
                      <th>Sources</th>
                      <th>Used</th>
                      <th>Number</th>
                    </tr>
                  </thead>
                  <tbody>
                    <tr>
                      <td>Infrared Thermometer</td>
                      <td>
                        <Field
                          type="checkbox"
                          name="sect5.weatherMonitoringSources.infraredThermometer.relied"
                          className="form-control"
                        />
                      </td>
                      <td>
                        <FormNumberInput
                          type="number"
                          name="sect5.weatherMonitoringSources.infraredThermometer.number"
                        />
                      </td>
                    </tr>
                    <tr>
                      <td>Meteorological Service</td>
                      <td>
                        <Field
                          type="checkbox"
                          name="sect5.weatherMonitoringSources.meteorologicalService.relied"
                          className="form-control"
                        />
                      </td>
                    </tr>
                    <tr>
                      <td>Fixed RWIS Stations</td>
                      <td>
                        <Field
                          type="checkbox"
                          name="sect5.weatherMonitoringSources.fixedRWISStations.relied"
                          className="form-control"
                        />
                      </td>
                      <td>
                        <FormNumberInput type="number" name="sect5.weatherMonitoringSources.fixedRWISStations.number" />
                      </td>
                    </tr>
                    <tr>
                      <td>Mobile RWIS mounted on vehicles</td>
                      <td>
                        <Field
                          type="checkbox"
                          name="sect5.weatherMonitoringSources.mobileRWISMounted.relied"
                          className="form-control"
                        />
                      </td>
                      <td>
                        <FormNumberInput type="number" name="sect5.weatherMonitoringSources.mobileRWISMounted.number" />
                      </td>
                    </tr>
                  </tbody>
                </Table>
              </Col>
            </Row>
          </Col>
        </Row>
        <Row className="my-4">
          <Col sm={4}>
            <b>Maintenance Decision Support</b>
          </Col>
        </Row>
        <Row className="my-4">
          <Col sm={1}>5.3</Col>
          <Col sm={7}>
            Indicate the type of system your organization relies on to help improve decision making for maintenance
            strategy, materials and application rate.
            <Row className="my-2">
              <Col>
                <Table bordered>
                  <thead>
                    <tr>
                      <th>Type</th>
                      <th>Used</th>
                      <th># of vehicles</th>
                    </tr>
                  </thead>
                  <tbody>
                    <tr>
                      <td>Automated Vehicle Location (AVL)</td>
                      <td>
                        <Field
                          type="checkbox"
                          name="sect5.maintenanceDecisionSupport.avl.relied"
                          className="form-control"
                        />
                      </td>
                      <td>
                        <FormNumberInput type="number" name="sect5.maintenanceDecisionSupport.avl.number" />
                      </td>
                    </tr>
                    <tr>
                      <td>Record of salt application rates</td>
                      <td>
                        <Field
                          type="checkbox"
                          name="sect5.maintenanceDecisionSupport.saltApplicationRates.relied"
                          className="form-control"
                        />
                      </td>
                      <td>
                        <FormNumberInput
                          type="number"
                          name="sect5.maintenanceDecisionSupport.saltApplicationRates.number"
                        />
                      </td>
                    </tr>
                    <tr>
                      <td>Use of a chart for application rates adapted to road or temperature conditions</td>
                      <td>
                        <Field
                          type="checkbox"
                          name="sect5.maintenanceDecisionSupport.applicationRateChart.relied"
                          className="form-control"
                        />
                      </td>
                      <td>
                        <FormNumberInput
                          type="number"
                          name="sect5.maintenanceDecisionSupport.applicationRateChart.number"
                        />
                      </td>
                    </tr>
                    <tr>
                      <td>Testing of Maintenance Decision Support System (MDSS)</td>
                      <td>
                        <Field
                          type="checkbox"
                          name="sect5.maintenanceDecisionSupport.testingMDSS.relied"
                          className="form-control"
                        />
                      </td>
                      <td>
                        <FormNumberInput type="number" name="sect5.maintenanceDecisionSupport.testingMDSS.number" />
                      </td>
                    </tr>
                  </tbody>
                </Table>
              </Col>
            </Row>
          </Col>
        </Row>
      </section>
      <section>
        <h4>Section 6: Snow Disposal</h4>
        <Row className="my-4">
          <Col>
            <Table bordered>
              <thead>
                <tr>
                  <th>Management of Snow</th>
                  <th>(Y/N)</th>
                  <th># of sites</th>
                </tr>
              </thead>
              <tbody>
                <tr>
                  <td>6.1 Does your organization perform snow disposal at a designated site?</td>
                  <td>
                    <Field type="checkbox" name="sect6.disposal.used" className="form-control" />
                  </td>
                  <td>
                    <FormNumberInput type="number" name="sect6.disposal.total" />
                  </td>
                </tr>
                <tr>
                  <td>6.2 Does your organization use snow melters?</td>
                  <td>
                    <Field type="checkbox" name="sect6.snowMelter.used" className="form-control" />
                  </td>
                </tr>
                <tr>
                  <td>6.3 Is the meltwater from snow melters discharged though the storm sewer system?</td>
                  <td>
                    <Field type="checkbox" name="sect6.meltwater.used" className="form-control" />
                  </td>
                </tr>
              </tbody>
            </Table>
          </Col>
        </Row>
      </section>
      <section>
        <h4>Section 7: Management of Salt Vulnerable Areas</h4>
        <Row className="my-4">
          <Col sm={1}>7.1</Col>
          <Col sm={7}>
            Has your organization completed an inventory of salt vulnerable areas within your Service Area?
          </Col>{' '}
          <Col>
            <div>
              <FormRadioInput id="completedInventory.no" name="sect7.completedInventory" value="no" label="No" />
              <FormRadioInput id="completedInventory.yes" name="sect7.completedInventory" value="yes" label="Yes" />
            </div>
          </Col>
        </Row>
        <Row className="my-4">
          <Col sm={1}>7.2</Col>
          <Col sm={7}>Do you have salt vulnerable areas within your Service Area? (provide list below)</Col>{' '}
          <Col>
            <div>
              <FormRadioInput id="setVulnerableAreas.no" name="sect7.setVulnerableAreas" value="no" label="No" />
              <FormRadioInput id="setVulnerableAreas.yes" name="sect7.setVulnerableAreas" value="yes" label="Yes" />
            </div>
          </Col>
        </Row>
        <Row className="my-4">
          <Col sm={1}>7.3</Col>
          <Col sm={7}>
            Has your organization prepared an action plan to prioritize areas where measures will be put in place?
          </Col>{' '}
          <Col>
            <div>
              <FormRadioInput id="actionPlanPrepared.no" name="sect7.actionPlanPrepared" value="no" label="No" />
              <FormRadioInput id="actionPlanPrepared.yes" name="sect7.actionPlanPrepared" value="yes" label="Yes" />
            </div>
          </Col>
        </Row>
        <Row className="my-4">
          <Col sm={1}>7.4</Col>
          <Col sm={7}>
            Did your organization implement supplementary and specific protection or mitigation measures to eliminate or
            reduce road salt impacts on vulnerable areas?
          </Col>{' '}
          <Col>
            <div>
              <FormRadioInput
                id="protectionMeasuresImplemented.no"
                name="sect7.protectionMeasuresImplemented"
                value="no"
                label="No"
              />
              <FormRadioInput
                id="protectionMeasuresImplemented.yes"
                name="sect7.protectionMeasuresImplemented"
                value="yes"
                label="Yes"
              />
            </div>
          </Col>
        </Row>
        <Row className="my-4">
          <Col sm={1}>7.5</Col>
          <Col sm={7}>
            Does your organization conduct environmental monitoring to measure impacts of road salts on vulnerable
            areas?
          </Col>{' '}
          <Col>
            <div>
              <FormRadioInput
                id="environmentalMonitoringConducted.no"
                name="sect7.environmentalMonitoringConducted"
                value="no"
                label="No"
              />
              <FormRadioInput
                id="environmentalMonitoringConducted.yes"
                name="sect7.environmentalMonitoringConducted"
                value="yes"
                label="Yes"
              />
            </div>
          </Col>
        </Row>
        <Row>
          <Col className="my-2">
            <h4>Types of Vulnerable Areas</h4>
            <Table bordered>
              <thead>
                <tr>
                  <td>Type of Vulnerability</td>
                  <td># of areas identified</td>
                  <td># of areas with protection measures</td>
                  <td># of areas with chloride monitoring</td>
                </tr>
              </thead>
              <tbody>
                <tr>
                  <td>Drinking Water (surface or ground water)</td>
                  <td>
                    <FormNumberInput type="number" name="sect7.typesOfVulnerableAreas.drinkingWater.areasIdentified" />
                  </td>
                  <td>
                    <FormNumberInput
                      type="number"
                      name="sect7.typesOfVulnerableAreas.drinkingWater.areasWithProtection"
                    />
                  </td>
                  <td>
                    <FormNumberInput
                      type="number"
                      name="sect7.typesOfVulnerableAreas.drinkingWater.areasWithChloride"
                    />
                  </td>
                </tr>
                <tr>
                  <td>Aquatic Life (lake and watercourse)</td>
                  <td>
                    <FormNumberInput type="number" name="sect7.typesOfVulnerableAreas.aquaticLife.areasIdentified" />
                  </td>
                  <td>
                    <FormNumberInput
                      type="number"
                      name="sect7.typesOfVulnerableAreas.aquaticLife.areasWithProtection"
                    />
                  </td>
                  <td>
                    <FormNumberInput type="number" name="sect7.typesOfVulnerableAreas.aquaticLife.areasWithChloride" />
                  </td>
                </tr>
                <tr>
                  <td>Wetlands and associated aquatic life</td>
                  <td>
                    <FormNumberInput type="number" name="sect7.typesOfVulnerableAreas.wetlands.areasIdentified" />
                  </td>
                  <td>
                    <FormNumberInput type="number" name="sect7.typesOfVulnerableAreas.wetlands.areasWithProtection" />
                  </td>
                  <td>
                    <FormNumberInput type="number" name="sect7.typesOfVulnerableAreas.wetlands.areasWithChloride" />
                  </td>
                </tr>
                <tr>
                  <td>Delimited areas with terrestrial fauna/flora</td>
                  <td>
                    <FormNumberInput type="number" name="sect7.typesOfVulnerableAreas.delimitedAreas.areasIdentified" />
                  </td>
                  <td>
                    <FormNumberInput
                      type="number"
                      name="sect7.typesOfVulnerableAreas.delimitedAreas.areasWithProtection"
                    />
                  </td>
                  <td>
                    <FormNumberInput
                      type="number"
                      name="sect7.typesOfVulnerableAreas.delimitedAreas.areasWithChloride"
                    />
                  </td>
                </tr>
                <tr>
                  <td>Valued Lands</td>
                  <td>
                    <FormNumberInput type="number" name="sect7.typesOfVulnerableAreas.valuedLands.areasIdentified" />
                  </td>
                  <td>
                    <FormNumberInput
                      type="number"
                      name="sect7.typesOfVulnerableAreas.valuedLands.areasWithProtection"
                    />
                  </td>
                  <td>
                    <FormNumberInput type="number" name="sect7.typesOfVulnerableAreas.valuedLands.areasWithChloride" />
                  </td>
                </tr>
              </tbody>
            </Table>
          </Col>
        </Row>
        <Row className="my-2">
          <Col>
            <h4>List the Vulnerable Areas in your Service Area: (insert additional rows as needed to table below)</h4>
            <FieldArray name="sect7.vulnerableAreas">
              {({ insert, remove }) => (
                <>
                  <Table bordered style={{ tableLayout: 'fixed' }}>
                    <thead>
                      <tr>
                        <th>Hwy #</th>
                        <th>Latitude</th>
                        <th>Longitude</th>
                        <th>Feature (ie: lake, stream)</th>
                        <th>Type</th>
                        <th>Type of Protection Measures (refer to Salt Management Plan)</th>
                        <th>Is Environmental monitoring in place? (Tick for Yes)</th>
                        <th>Comments</th>
                      </tr>
                    </thead>
                    <tbody>
                      {formValues.sect7.vulnerableAreas.map((area, index) => (
                        <tr key={index}>
                          <td>
                            <FormInput name={`sect7.vulnerableAreas.${index}.highwayNumber`} type="text" />
                          </td>
                          <td>
                            <FormNumberInput name={`sect7.vulnerableAreas.${index}.latitude`} type="number" />
                          </td>
                          <td>
                            <FormNumberInput name={`sect7.vulnerableAreas.${index}.longitude`} type="number" />
                          </td>
                          <td>
                            <FormInput name={`sect7.vulnerableAreas.${index}.feature`} type="text" />
                          </td>
                          <td>
                            <Field name={`sect7.vulnerableAreas.${index}.type`} as="select">
                              <option value="">Select an option</option>
                              <option value="drinkingwater">Drinking Water</option>
                              <option value="aquatic">Aquatic Life</option>
                              <option value="wetlands">Wetlands</option>
                              <option value="delimited">Delimited</option>
                              <option value="valuedland">Valued Land</option>
                            </Field>
                          </td>
                          <td>
                            <FormInput name={`sect7.vulnerableAreas.${index}.protectioneasures`} type="text" />
                          </td>
                          <td>
                            <Field
                              name={`sect7.vulnerableAreas.${index}.environmentalMonitoring`}
                              type="checkbox"
                              className="form-control"
                            />
                          </td>
                          <td>
                            <FormInput name={`sect7.vulnerableAreas.${index}.comments`} type="text" />
                          </td>
                          <td>
                            <Button type="button" onClick={() => remove(index)}>
                              Remove
                            </Button>
                          </td>
                        </tr>
                      ))}
                    </tbody>
                  </Table>
                  <Button type="button" onClick={() => insert(formValues.sect7.vulnerableAreas.length, {})}>
                    Add vulnerable area
                  </Button>
                </>
              )}
            </FieldArray>
          </Col>
        </Row>
      </section>
      <section>
        <Col>
          <h4>Appendix A</h4>
          <Table bordered>
            <thead>
              <tr>
                <th>Material Storage Facilities</th>
                <th>Identified</th>
                <th>Achieved</th>
              </tr>
            </thead>
            <tbody>
              <tr>
                <td colSpan={3}>
                  Quality of salt supplied is verified through the following steps:
                  <ol>
                    <li>a quality control program is put in place</li>
                    <li>humidity test on salt delivered</li>
                    <li>granulometry and</li>
                    <li>chemical analysis for salt concentration</li>
                  </ol>
                </td>
              </tr>
              {Object.entries(formValues.appendix.materialStorage).map(([key, _value], index) => (
                <tr key={index}>
                  <td>{materialStorageAppendixLabel.materialStorage[key]}</td>
                  <td>
                    <FormNumberInput
                      type="number"
                      name={`appendix.materialStorage.${key}.identified`}
                      className="form-control"
                    />
                  </td>
                  <td>
                    <FormNumberInput
                      type="number"
                      name={`appendix.materialStorage.${key}.achieved`}
                      className="form-control"
                    />
                  </td>
                </tr>
              ))}
            </tbody>
          </Table>
          <Table bordered>
            <thead>
              <tr>
                <th>Salt Application</th>
                <th>Identified</th>
                <th>Achieved</th>
              </tr>
            </thead>
            <tbody>
              {Object.entries(formValues.appendix.saltApplication).map(([key, value], index) => (
                <tr key={index}>
                  <td>{materialStorageAppendixLabel.saltApplication[key]}</td>
                  {key === 'reviewSaltUse' ? null : (
                    <>
                      <td>
                        <FormNumberInput
                          type="number"
                          name={`appendix.saltApplication.${key}.identified`}
                          className="form-control"
                        />
                      </td>
                      <td>
                        <FormNumberInput
                          type="number"
                          name={`appendix.saltApplication.${key}.achieved`}
                          className="form-control"
                        />
                      </td>
                    </>
                  )}
                </tr>
              ))}
            </tbody>
          </Table>
          <Table bordered>
            <thead>
              <tr>
                <th>Snow Disposal</th>
                <th>Identified</th>
                <th>Achieved</th>
              </tr>
            </thead>
            <tbody>
              {Object.entries(formValues.appendix.snowDisposal).map(([key, value], index) => (
                <tr key={index}>
                  <td>{materialStorageAppendixLabel.snowDisposal[key]}</td>
                  <td>
                    <FormNumberInput
                      type="number"
                      name={`appendix.snowDisposal.${key}.identified`}
                      className="form-control"
                    />
                  </td>
                  <td>
                    <FormNumberInput
                      type="number"
                      name={`appendix.snowDisposal.${key}.achieved`}
                      className="form-control"
                    />
                  </td>
                </tr>
              ))}
            </tbody>
          </Table>
          <Table bordered>
            <thead>
              <tr>
                <th>Vulnerable Areas</th>
                <th>Identified</th>
                <th>Achieved</th>
              </tr>
            </thead>
            <tbody>
              {Object.entries(formValues.appendix.vulnerableAreas).map(([key, value], index) => (
                <tr key={index}>
                  <td>{materialStorageAppendixLabel.vulnerableAreas[key]}</td>
                  <td>
                    <FormNumberInput
                      type="number"
                      name={`appendix.vulnerableAreas.${key}.identified`}
                      className="form-control"
                    />
                  </td>
                  <td>
                    <FormNumberInput
                      type="number"
                      name={`appendix.vulnerableAreas.${key}.achieved`}
                      className="form-control"
                    />
                  </td>
                </tr>
              ))}
            </tbody>
          </Table>
        </Col>
      </section>
    </TooltipProvider>
  );
};

const mapStateToProps = (state) => {
  return {
    currentUser: state.user.current,
  };
};

export default connect(mapStateToProps)(AddSaltReportFormFields);
