import React, { useEffect, useState } from 'react';

import * as Yup from 'yup';
import SingleDateField from '../ui/SingleDateField';
import MultiSelect from '../ui/MultiSelect';
import SingleDropdownField from '../ui/SingleDropdownField';
import PageSpinner from '../ui/PageSpinner';
import FieldSet from '../ui/FieldSet';

import { FormRow, FormInput, FormCheckboxInput, FormRadioInput } from './FormInputs';

import { Row, Col, FormGroup, Label, Input, Table, Button } from 'reactstrap';
import { isInteger } from 'lodash';
import { isValueEmpty, isValueNotEmpty, toStringOrEmpty, toStringWithCommasOrEmpty, isValidDecimal } from '../../utils';
import { Field, FieldArray } from 'formik';

const defaultValues = {
  serviceArea: '',
  contactName: '',
  telephone: '',
  email: '',
  sect1: {
    developed: '',
    materialIdentified: null,
    materialAchieved: null,
    saltIdentified: null,
    saltAchieved: null,
    training: {
      manager: false,
      supervisor: false,
      operator: false,
      mechanical: false,
      patroller: false,
    },
  },
  sect2: {
    roadTotalLength: '',
    saltTotalDays: '',
  },
  sect3: {
    deicer: { nacl: null, mgcl2: null, cacl2: null, acetate: null },
    treatedAbrasives: { sandStoneDust: null, nacl: null, mgcl2: null, cacl2: null },
    prewetting: { nacl: null, mgcl2: null, cacl2: null, acetate: null, nonchloride: null },
    pretreatment: { nacl: null, mgcl2: null, cacl2: null, acetate: null, nonchloride: null },
    antiicing: { nacl: null, mgcl2: null, cacl2: null, acetate: null, nonchloride: null },
    multiChlorideA: {
      litres: '',
      naClPercentage: '',
      mgCl2Percentage: '',
      caCl2Percentage: '',
    },
    multiChlorideB: {
      litres: '',
      naClPercentage: '',
      mgCl2Percentage: '',
      caCl2Percentage: '',
    },
  },
  sect4: {
    saltStorageTotal: null,
    stockpiles: [
      {
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
      },
    ],
    practices: [
      {
        practice: 'All materials are handled in a designated area characterized by an impermeable surface',
        hasPlan: false,
        numSites: '',
      },
      { practice: 'Equipment to prevent overloading of trucks', numSites: '', hasPlan: false },
      {
        practice: 'System for collection and/or treatment of wastewater from cleaning of trucks',
        numSites: '',
        hasPlan: false,
      },
      { practice: 'Control and diversion of external waters (non salt impacted', numSites: '', hasPlan: false },
      {
        practice: 'Drainage inside with collection systems for runoff of salt contaminated waters',
        numSites: '',
        hasPlan: false,
      },
      {
        practice: 'Specify discharge point into a municipal sewer system',
        numSites: '',
        hasPlan: false,
      },
      {
        practice: 'Specify discharge point into a containment for removal',
        numSites: '',
        hasPlan: false,
      },
      {
        practice: 'Specify discharge point into a watercourse',
        numSites: '',
        hasPlan: false,
      },
      {
        practice: 'Specify discharge point into (other)',
        numSites: '',
        hasPlan: false,
      },
      {
        practice: 'Ongoing cleanup of the site surfaces, and spilled material is swept up quickly',
        numSites: '',
        hasPlan: false,
      },
      { practice: 'Risk management and emergency measures plans are in place', numSites: '', hasPlan: false },
    ],
  },
  sect5: {
    numberOfVehicles: '',
    vehiclesForSaltApplication: '',
    vehiclesWithConveyors: '',
    vehiclesWithPreWettingEquipment: '',
    vehiclesForDLA: '',
    calibrationFrequency: '',
    weatherMonitoringSources: {
      infraredThermometer: {
        relied: false,
        number: '',
      },
      meteorologicalService: {
        relied: false,
      },
      fixedRWISStations: {
        relied: false,
        number: '',
      },
      mobileRWISMounted: {
        relied: false,
        number: '',
      },
    },
    maintenanceDecisionSupport: {
      AVL: {
        relied: false,
        number: '',
      },
      saltApplicationRates: {
        relied: false,
        number: '',
      },
      applicationRateChart: {
        relied: false,
        number: '',
      },
      testingMDSS: {
        relied: false,
        number: '',
      },
    },
  },
  sect6: {
    snowDisposalSite: {
      used: false,
      number: ''
    },
    snowMelters: {
      used: false,
    },
    meltwaterDisposalMethod: {
      used: false,
    },
  },
  sect7: {
    completedInventory: false,
    setVulnerableAreas: false,
    actionPlanPrepared: false,
    protectionMeasuresImplemented: false,
    environmentalMonitoringConducted: false,
    typesOfVulnerableAreas: {
      drinkingWater: {
        areasIdentified: '',
        areasWithProtection: '',
        areasWithChloride: '',
      },
      aquaticLife: {
        areasIdentified: '',
        areasWithProtection: '',
        areasWithChloride: '',
      },
      wetlands: {
        areasIdentified: '',
        areasWithProtection: '',
        areasWithChloride: '',
      },
      delimitedAreas: {
        areasIdentified: '',
        areasWithProtection: '',
        areasWithChloride: '',
      },
      valuedLands: {
        areasIdentified: '',
        areasWithProtection: '',
        areasWithChloride: '',
      },
    },
    vulnerableAreas: [
      {
        highway: '',
        latitude: '',
        longitude: '',
        feature: '',
        type: '',
        protectionMeasures: '',
        monitoringInPlace: false,
      },
    ],
  },
};

const validationSchema = Yup.object({
  serviceArea: Yup.string().required('Service area is required'),
  contactName: Yup.string().required('Contact name is required'),
  telephone: Yup.string()
    .matches(/^[0-9]+$/, 'Telephone must be a number')
    .required('Telephone is required'),
  email: Yup.string().email('Invalid email format').required('Email is required'),
  sect1: Yup.object({
    developed: Yup.string().required(),
    materialIdentified: Yup.number(),
    materialAchieved: Yup.number(),
    saltIdentified: Yup.number(),
    saltAchieved: Yup.number(),
  }),
  sect2: Yup.object({
    roadTotalLength: Yup.number().required('Road total length is required'),
  }),
  sect3: Yup.object({
    deicer: Yup.object({
      nacl: Yup.number().nullable(true),
      mgcl2: Yup.number().nullable(true),
      cacl2: Yup.number().nullable(true),
      acetate: Yup.number().nullable(true),
    }),
    treatedAbrasives: Yup.object({
      sandStoneDust: Yup.number().nullable(true),
      nacl: Yup.number().nullable(true),
      mgcl2: Yup.number().nullable(true),
      cacl2: Yup.number().nullable(true),
    }),
    prewetting: Yup.object({
      nacl: Yup.number().nullable(true),
      mgcl2: Yup.number().nullable(true),
      cacl2: Yup.number().nullable(true),
      acetate: Yup.number().nullable(true),
      nonchloride: Yup.number().nullable(true),
    }),
    pretreatment: Yup.object({
      nacl: Yup.number().nullable(true),
      mgcl2: Yup.number().nullable(true),
      cacl2: Yup.number().nullable(true),
      acetate: Yup.number().nullable(true),
      nonchloride: Yup.number().nullable(true),
    }),
    antiicing: Yup.object({
      nacl: Yup.number().nullable(true),
      mgcl2: Yup.number().nullable(true),
      cacl2: Yup.number().nullable(true),
      acetate: Yup.number().nullable(true),
      nonchloride: Yup.number().nullable(true),
    }),
  }),
  sect4: Yup.object({
    saltStorageTotal: Yup.number().nullable().required('Required'),
    stockpiles: Yup.array().of(
      Yup.object({
        siteName: Yup.string(),
        motiOwned: Yup.boolean(),
        roadSalts: Yup.object({
          stockpilesTotal: Yup.number().nullable(),
          onImpermeableSurface: Yup.number().nullable(),
          underPermanentRoof: Yup.number().nullable(),
          underTarp: Yup.number().nullable(),
        }),
        treatedAbrasives: Yup.object({
          stockpilesTotal: Yup.number().nullable(),
          onImpermeableSurface: Yup.number().nullable(),
          underPermanentRoof: Yup.number().nullable(),
          underTarp: Yup.number().nullable(),
        }),
      })
    ),
  }),
  sect5: Yup.object({}), // Define validations for sect5
  sect6: Yup.object({}), // Define validations for sect6
  sect7: Yup.object({
    completedInventory: Yup.string(),
    setVulnerableAreas: Yup.string(),
    actionPlanPrepared: Yup.string(),
    protectionMeasuresImplemented: Yup.string(),
    environmentalMonitoringConducted: Yup.string(),
    vulnerableAreas: Yup.array().of(
      Yup.object({
        highway: Yup.string(),
        latitude: Yup.string(),
        longitude: Yup.string(),
        feature: Yup.string(),
        type: Yup.string(),
        protectionMeasures: Yup.string(),
        monitoringInPlace: Yup.boolean(),
      })
    ),
  }),
});

const AddSaltReportFormFields = ({ setInitialValues, formValues, setValidationSchema }) => {
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    setLoading(true);
    const defaultValidationSchema = validationSchema.shape({});
    setValidationSchema(defaultValidationSchema);
    setInitialValues(defaultValues);
    setLoading(false);
  }, []);

  if (loading || formValues === null) return <PageSpinner />;

  setInitialValues(defaultValues);

  return (
    <React.Fragment>
      <Row>
        <Col>
          <FormRow name="serviceArea" label="Service Area">
            <FormInput type="text" name="serviceArea" label="Service Area" />
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
              <FormRadioInput id="developed.no" name="sect1.developed" value="no" label="No" />
              <FormRadioInput id="developed.yes" name="sect1.developed" value="yes" label="Yes" />
            </div>
          </Col>
        </Row>
        <Row className="my-4">
          <Col sm={1}>1.2</Col>
          <Col sm={7}>In the past year, did your organization conduct a review of its salt management plan?</Col>
          <Col>
            <div>
              <FormRadioInput id="reviewConducted.no" name="sect1.reviewConducted" value="no" label="No" />
              <FormRadioInput id="reviewConducted.yes" name="sect1.reviewConducted" value="yes" label="Yes" />
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
        <Row className="my-4">
          <Col>
            <div>
              Manager(s)
              <FormRadioInput id="manager.no" name="sect1.manager" value="no" label="No" />
              <FormRadioInput id="manager.yes" name="sect1.manager" value="yes" label="Yes" />
            </div>
          </Col>
          <Col>
            <div>
              Supervisor(s)
              <FormRadioInput id="supervisor.no" name="sect1.supervisor" value="no" label="No" />
              <FormRadioInput id="supervisor.yes" name="sect1.supervisor" value="yes" label="Yes" />
            </div>
          </Col>
          <Col>
            <div>
              Operator(s)
              <FormRadioInput id="operator.no" name="sect1.operator" value="no" label="No" />
              <FormRadioInput id="operator.yes" name="sect1.operator" value="yes" label="Yes" />
            </div>
          </Col>
          <Col>
            <div>
              Mechanical
              <FormRadioInput id="mechanical.no" name="sect1.mechanical" value="no" label="No" />
              <FormRadioInput id="mechanical.yes" name="sect1.mechanical" value="yes" label="Yes" />
            </div>
          </Col>
          <Col>
            <div>
              Patroller(s)
              <FormRadioInput id="patroller.no" name="sect1.patroller" value="no" label="No" />
              <FormRadioInput id="patroller.yes" name="sect1.patroller" value="yes" label="Yes" />
            </div>
          </Col>
        </Row>
        <Row className="my-4">
          <Col>
            <Row>
              <Col sm={1}>1.5</Col>
              <Col>
                Indicate the number of objectives<sup>1</sup> that were identified and achieved for this year in your
                salt management plan within the following areas: (refer to Appendix A for a sample list of objectives)
              </Col>
            </Row>
            <Row className="my-2">
              <Col>
                <Table bordered style={{ tableLayout: 'fixed' }}>
                  <thead>
                    <tr>
                      <th rowSpan={2}>Areas for Improvement</th>
                      <th colSpan={2}>Number of Objectives for Winter 2022/23</th>
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
                        <FormGroup>
                          <FormInput type="number" name="sect1.materialIdentified" />
                        </FormGroup>
                      </td>
                      <td>
                        <FormGroup>
                          <FormInput type="number" name="sect1.materialAchieved" />
                        </FormGroup>
                      </td>
                    </tr>
                    <tr>
                      <td>Salt Application</td>
                      <td>
                        <FormGroup>
                          <FormInput type="number" name="sect1.saltIdentified" />
                        </FormGroup>
                      </td>
                      <td>
                        <FormGroup>
                          <FormInput type="number" name="sect1.saltAchieved" />
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
            <FormInput
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
            <FormInput type="number" name="sect2.saltTotalDays" placeholder="days" />
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
                Indicate the number of objectives<sup>1</sup> that were identified and achieved for this year in your
                salt management plan within the following areas: (refer to Appendix A for a sample list of objectives)
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
                        <FormInput type="number" name="sect3.deicer.nacl" />
                      </td>
                    </tr>
                    <tr>
                      <th>Magnesium chloride (MgCl2)</th>
                      <td>
                        <FormInput type="number" name="sect3.deicer.mgcl2" />
                      </td>
                    </tr>
                    <tr>
                      <th>Calcium chloride (CaCl2)</th>
                      <td>
                        <FormInput type="number" name="sect3.deicer.cacl2" />
                      </td>
                    </tr>
                    <tr>
                      <th>Acetate2</th>
                      <td>
                        <FormInput type="number" name="sect3.deicer.acetate" />
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
                        <FormInput type="number" name="sect3.treatedAbrasives.sandStoneDust" />
                      </td>
                    </tr>
                    <tr>
                      <th>Sodium chloride (NaCl)</th>
                      <td>
                        <FormInput type="number" name="sect3.treatedAbrasives.nacl" />
                      </td>
                    </tr>
                    <tr>
                      <th>Magnesium chloride (MgCl2)</th>
                      <td>
                        <FormInput type="number" name="sect3.treatedAbrasives.mgcl2" />
                      </td>
                    </tr>
                    <tr>
                      <th>Calcium chloride (CaCl2)</th>
                      <td>
                        <FormInput type="number" name="sect3.treatedAbrasives.cacl2" />
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
                        <FormInput type="number" name="sect3.prewetting.nacl" />
                      </td>
                    </tr>
                    <tr>
                      <th>Magnesium chloride (MgCl2)</th>
                      <td>
                        <FormInput type="number" name="sect3.prewetting.mgcl2" />
                      </td>
                    </tr>
                    <tr>
                      <th>Calcium chloride (CaCl2)</th>
                      <td>
                        <FormInput type="number" name="sect3.prewetting.cacl2" />
                      </td>
                    </tr>
                    <tr>
                      <th>Acetate2</th>
                      <td>
                        <FormInput type="number" name="sect3.prewetting.acetate" />
                      </td>
                    </tr>
                    <tr>
                      <th>Non-chloride organic products3</th>
                      <td>
                        <FormInput type="number" name="sect3.prewetting.nonchloride" />
                      </td>
                    </tr>
                    <tr>
                      <th rowSpan={5}>
                        Pre-treatment Liquid Concentrated liquid product added to the solid de-icer and the abrasive at
                        the time it is stockpiled at the storage site or added by the supplier before delivery
                      </th>
                      <th>Sodium chloride (NaCl)</th>
                      <td>
                        <FormInput type="number" name="sect3.pretreatment.nacl" />
                      </td>
                    </tr>
                    <tr>
                      <th>Magnesium chloride (MgCl2)</th>
                      <td>
                        <FormInput type="number" name="sect3.pretreatment.mgcl2" />
                      </td>
                    </tr>
                    <tr>
                      <th>Calcium chloride (CaCl2)</th>
                      <td>
                        <FormInput type="number" name="sect3.pretreatment.cacl2" />
                      </td>
                    </tr>
                    <tr>
                      <th>Acetate2</th>
                      <td>
                        <FormInput type="number" name="sect3.pretreatment.acetate" />
                      </td>
                    </tr>
                    <tr>
                      <th>Non-chloride organic products3</th>
                      <td>
                        <FormInput type="number" name="sect3.pretreatment.nonchloride" />
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
                        <FormInput type="number" name="sect3.antiicing.nacl" />
                      </td>
                    </tr>
                    <tr>
                      <th>Magnesium chloride (MgCl2)</th>
                      <td>
                        <FormInput type="number" name="sect3.antiicing.mgcl2" />
                      </td>
                    </tr>
                    <tr>
                      <th>Calcium chloride (CaCl2)</th>
                      <td>
                        <FormInput type="number" name="sect3.antiicing.cacl2" />
                      </td>
                    </tr>
                    <tr>
                      <th>Acetate2</th>
                      <td>
                        <FormInput type="number" name="sect3.antiicing.acetate" />
                      </td>
                    </tr>
                    <tr>
                      <th>Non-chloride organic products3</th>
                      <td>
                        <FormInput type="number" name="sect3.antiicing.nonchloride" />
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
                If your organization uses multi-chloride liquids4, specify litres used and concentration for each salt
                in the liquid:
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
                        <Field name="multiChlorideA.litres" type="number" as={Input} />
                      </td>
                      <td>
                        <Field name="multiChlorideA.naClPercentage" type="number" as={Input} />
                      </td>
                      <td>
                        <Field name="multiChlorideA.mgCl2Percentage" type="number" as={Input} />
                      </td>
                      <td>
                        <Field name="multiChlorideA.caCl2Percentage" type="number" as={Input} />
                      </td>
                    </tr>
                    {/* Multi-chloride B */}
                    <tr>
                      <td>Multi-chloride A</td>
                      <td>
                        <Field name="multiChlorideB.litres" type="number" as={Input} />
                      </td>
                      <td>
                        <Field name="multiChlorideB.naClPercentage" type="number" as={Input} />
                      </td>
                      <td>
                        <Field name="multiChlorideB.mgCl2Percentage" type="number" as={Input} />
                      </td>
                      <td>
                        <Field name="multiChlorideB.caCl2Percentage" type="number" as={Input} />
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
            <FormInput type="number" name="sect4.saltStorageTotal" />
          </Col>
        </Row>
        <Row className="my-4">
          <Col>
            <Row>
              <Col sm={1}>4.2</Col>
              <Col>
                Indicate the number of objectives<sup>1</sup> that were identified and achieved for this year in your
                salt management plan within the following areas: (refer to Appendix A for a sample list of objectives)
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
                                <FormInput type="number" name={`sect4.stockpiles.${index}.siteName`} />
                              </td>
                              <td>
                                <FormCheckboxInput type="checkbox" name={`sect4.stockpiles.${index}.motiOwned`} />
                              </td>
                              <td>
                                <FormInput type="number" name={`sect4.stockpiles.${index}.roadSalts.stockpilesTotal`} />
                              </td>
                              <td>
                                <FormInput
                                  type="number"
                                  name={`sect4.stockpiles.${index}.roadSalts.onImpermeableSurface`}
                                />
                              </td>
                              <td>
                                <FormInput
                                  type="number"
                                  name={`sect4.stockpiles.${index}.roadSalts.underPermanentRoof`}
                                />
                              </td>
                              <td>
                                <FormInput type="number" name={`sect4.stockpiles.${index}.roadSalts.underTarp`} />
                              </td>
                              <td>
                                <FormInput
                                  type="number"
                                  name={`sect4.stockpiles.${index}.treatedAbrasives.stockpilesTotal`}
                                />
                              </td>
                              <td>
                                <FormInput
                                  type="number"
                                  name={`sect4.stockpiles.${index}.treatedAbrasives.onImpermeableSurface`}
                                />
                              </td>
                              <td>
                                <FormInput
                                  type="number"
                                  name={`sect4.stockpiles.${index}.treatedAbrasives.underPermanentRoof`}
                                />
                              </td>
                              <td>
                                <FormInput
                                  type="number"
                                  name={`sect4.stockpiles.${index}.treatedAbrasives.underTarp`}
                                />
                              </td>
                              <td>
                                <Button type="button" onClick={() => remove(index)}>
                                  -
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
                            motiOwned: '',
                            roadSalts: {},
                            treatedAbrasives: {},
                          })
                        }
                      >
                        Add Row
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
                Indicate the number of objectives<sup>1</sup> that were identified and achieved for this year in your
                salt management plan within the following areas: (refer to Appendix A for a sample list of objectives)
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
                    {formValues.sect4.practices.map((item, index) => (
                      <tr key={index}>
                        <td>{item.practice}</td>
                        <td>
                          <Field name={`practices[${index}].hasPlan`} type="checkbox" className="form-control" />
                        </td>
                        <td>
                          <Field name={`practices[${index}].numSites`} type="number" className="form-control" />
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
                <Field name="sect5.totalVehicles" type="number" as={Input} />
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
                        <Field name="sect5.vehiclesForSaltApplication" type="number" as={Input} />
                      </td>
                    </tr>
                    <tr>
                      <td>Vehicles with conveyors and ground speed sensor electronic controller</td>
                      <td>
                        <Field name="sect5.vehiclesWithConveyors" type="number" as={Input} />
                      </td>
                    </tr>
                    <tr>
                      <td>Vehicles equipped with pre-wetting equipment</td>
                      <td>
                        <Field name="sect5.vehiclesWithPreWettingEquipment" type="number" as={Input} />
                      </td>
                    </tr>
                    <tr>
                      <td>Vehicles designed for direct liquid application (DLA)</td>
                      <td>
                        <Field name="sect5.vehiclesForDLA" type="number" as={Input} />
                      </td>
                    </tr>
                  </tbody>
                </Table>
              </Col>
            </Row>
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
                          value="Infrared Thermometer"
                          className="form-control"
                        />
                      </td>
                      <td>
                        <Field
                          type="number"
                          name="sect5.weatherMonitoringSources.infraredThermometer.number"
                          as={Input}
                        />
                      </td>
                    </tr>
                    <tr>
                      <td>Meteorological Service</td>
                      <td>
                        <Field
                          type="checkbox"
                          name="sect5.weatherMonitoringSources.meteorologicalService.relied"
                          value="Infrared Thermometer"
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
                          value="Infrared Thermometer"
                          className="form-control"
                        />
                      </td>
                      <td>
                        <Field
                          type="number"
                          name="sect5.weatherMonitoringSources.fixedRWISStations.number"
                          as={Input}
                        />
                      </td>
                    </tr>
                    <tr>
                      <td>Mobile RWIS mounted on vehicles</td>
                      <td>
                        <Field
                          type="checkbox"
                          name="sect5.weatherMonitoringSources.mobileRWISMounted.relied"
                          value="Infrared Thermometer"
                          className="form-control"
                        />
                      </td>
                      <td>
                        <Field
                          type="number"
                          name="sect5.weatherMonitoringSources.mobileRWISMounted.number"
                          as={Input}
                        />
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
                          name="sect5.weatherMonitoringSources.AVL.relied"
                          value="Infrared Thermometer"
                          className="form-control"
                        />
                      </td>
                      <td>
                        <Field type="number" name="sect5.weatherMonitoringSources.AVL.number" as={Input} />
                      </td>
                    </tr>
                    <tr>
                      <td>Record of salt application rates</td>
                      <td>
                        <Field
                          type="checkbox"
                          name="sect5.weatherMonitoringSources.saltApplicationRates.relied"
                          value="Infrared Thermometer"
                          className="form-control"
                        />
                      </td>
                      <td>
                        <Field
                          type="number"
                          name="sect5.weatherMonitoringSources.saltApplicationRates.number"
                          as={Input}
                        />
                      </td>
                    </tr>
                    <tr>
                      <td>Use of a chart for application rates adapted to road or temperature conditions</td>
                      <td>
                        <Field
                          type="checkbox"
                          name="sect5.weatherMonitoringSources.applicationRateChart.relied"
                          value="Infrared Thermometer"
                          className="form-control"
                        />
                      </td>
                      <td>
                        <Field
                          type="number"
                          name="sect5.weatherMonitoringSources.applicationRateChart.number"
                          as={Input}
                        />
                      </td>
                    </tr>
                    <tr>
                      <td>Testing of Maintenance Decision Support System (MDSS)</td>
                      <td>
                        <Field
                          type="checkbox"
                          name="sect5.weatherMonitoringSources.testingMDSS.relied"
                          value="Infrared Thermometer"
                          className="form-control"
                        />
                      </td>
                      <td>
                        <Field type="number" name="sect5.weatherMonitoringSources.testingMDSS.number" as={Input} />
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
                    <Field
                      type="checkbox"
                      name="sect6.snowDisposalSite.used"
                      value="Infrared Thermometer"
                      className="form-control"
                    />
                  </td>
                  <td>
                    <Field type="number" name="sect6.snowDisposalSite.number" as={Input} />
                  </td>
                </tr>
                <tr>
                  <td>6.2 Does your organization use snow melters?</td>
                  <td>
                    <Field
                      type="checkbox"
                      name="sect6.snowMelters.used"
                      value="Infrared Thermometer"
                      className="form-control"
                    />
                  </td>
                </tr>
                <tr>
                  <td>6.3 Is the meltwater from snow melters discharged though the storm sewer system?</td>
                  <td>
                    <Field
                      type="checkbox"
                      name="sect6.meltwaterDisposalMethod.used"
                      value="Infrared Thermometer"
                      className="form-control"
                    />
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
          <Col className='my-2'>
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
                  <td><Field name='sect7.typesOfVulnerableAreas.drinkingWater.areasIdentified' as={Input} /></td>
                  <td><Field name='sect7.typesOfVulnerableAreas.drinkingWater.areasWithProtection' as={Input} /></td>
                  <td><Field name='sect7.typesOfVulnerableAreas.drinkingWater.areasWithChloride' as={Input} /></td>
                </tr>
                <tr>
                  <td>Aquatic Life (lake and watercourse)</td>
                  <td><Field name='sect7.typesOfVulnerableAreas.aquaticLife.areasIdentified' as={Input} /></td>
                  <td><Field name='sect7.typesOfVulnerableAreas.aquaticLife.areasWithProtection' as={Input} /></td>
                  <td><Field name='sect7.typesOfVulnerableAreas.aquaticLife.areasWithChloride' as={Input} /></td>
                </tr>
                <tr>
                  <td>Wetlands and associated aquatic life</td>
                  <td><Field name='sect7.typesOfVulnerableAreas.wetlands.areasIdentified' as={Input} /></td>
                  <td><Field name='sect7.typesOfVulnerableAreas.wetlands.areasWithProtection' as={Input} /></td>
                  <td><Field name='sect7.typesOfVulnerableAreas.wetlands.areasWithChloride' as={Input} /></td>
                </tr>
                <tr>
                  <td>Delimited areas with terrestrial fauna/flora</td>
                  <td><Field name='sect7.typesOfVulnerableAreas.delimitedAreas.areasIdentified' as={Input} /></td>
                  <td><Field name='sect7.typesOfVulnerableAreas.delimitedAreas.areasWithProtection' as={Input} /></td>
                  <td><Field name='sect7.typesOfVulnerableAreas.delimitedAreas.areasWithChloride' as={Input} /></td>
                </tr>
                <tr>
                  <td>Valued Lands</td>
                  <td><Field name='sect7.typesOfVulnerableAreas.valuedLands.areasIdentified' as={Input} /></td>
                  <td><Field name='sect7.typesOfVulnerableAreas.valuedLands.areasWithProtection' as={Input} /></td>
                  <td><Field name='sect7.typesOfVulnerableAreas.valuedLands.areasWithChloride' as={Input} /></td>
                </tr>
              </tbody>
            </Table>
          </Col>
        </Row>
        <Row className="my-2">
          <Col>
            <FieldArray name="sect7.vulnerableAreas">
              {({ insert, remove }) => (
                <>
                  <Table bordered style={{ tableLayout: 'fixed' }}>
                    <thead>
                      <tr>
                        <th>Hwy #</th>
                        <th>Latitude</th>
                        <th>Longitude</th>
                        <th>Feature</th>
                        <th>Type</th>
                        <th>Protection Measures</th>
                        <th>Environmental Monitoring in Place?</th>
                      </tr>
                    </thead>
                    <tbody>
                      {formValues.sect7.vulnerableAreas.map((area, index) => (
                        <tr key={index}>
                          <td>
                            <FormInput name={`sect7.vulnerableAreas.${index}.highway`} type="text" />
                          </td>
                          <td>
                            <FormInput name={`sect7.vulnerableAreas.${index}.latitude`} type="text" />
                          </td>
                          <td>
                            <FormInput name={`sect7.vulnerableAreas.${index}.longitude`} type="text" />
                          </td>
                          <td>
                            <FormInput name={`sect7.vulnerableAreas.${index}.feature`} type="text" />
                          </td>
                          <td>
                            <Field name={`sect7.vulnerableAreas.${index}.type`} as="select">
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
                            <FormCheckboxInput
                              name={`sect7.vulnerableAreas.${index}.monitoringInPlace`}
                              type="checkbox"
                            />
                          </td>
                          <td>
                            <Button type="button" onClick={() => remove(index)}>
                              -
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
    </React.Fragment>
  );
};

export default AddSaltReportFormFields;
