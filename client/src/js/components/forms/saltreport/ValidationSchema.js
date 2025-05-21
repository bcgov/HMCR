import * as Yup from 'yup';

// Custom validator for decimal with dynamic precision
const decimalWithPrecision = (precision) => {
  return Yup.number()
    .nullable(true)
    .max(99999999.99, 'Value must be less than or equal to 99,999,999.99')
    .test(
      'is-decimal',
      `Invalid format: number must have no more than ${precision} decimal places and be non-negative`,
      (value) => value === null || (value >= 0 && new RegExp(`^\\d+(\\.\\d{1,${precision}})?$`).test(value))
    );
};

// Custom validator for coordinate with dynamic precision
const coordinatePrecision = (precision) => {
  return Yup.number()
    .nullable(true)
    .test('is-valid-lat-lon', `Invalid format: number must have no more than ${precision} decimal places`, (value) => {
      if (value === null || value === undefined) {
        return true; // Allow null values
      }
      // Check if the value matches the precision requirement
      return new RegExp(`^-?\\d+(\\.\\d{1,${precision}})?$`).test(value);
    });
};

const stockpileSchema = Yup.object().shape({
  siteName: Yup.string().nullable(true),
  motiOwned: Yup.boolean().nullable(true),
  roadSalts: Yup.object()
    .shape({
      stockpilesTotal: Yup.number().integer().min(0, 'Cannot be negative').nullable(true),
      onImpermeableSurface: Yup.number().integer().min(0, 'Cannot be negative').nullable(true),
      underPermanentRoof: Yup.number().integer().min(0, 'Cannot be negative').nullable(true),
      underTarp: Yup.number().integer().min(0, 'Cannot be negative').nullable(true),
    })
    .nullable(true),
  treatedAbrasives: Yup.object()
    .shape({
      stockpilesTotal: Yup.number().integer().min(0, 'Cannot be negative').nullable(true),
      onImpermeableSurface: Yup.number().integer().min(0, 'Cannot be negative').nullable(true),
      underPermanentRoof: Yup.number().integer().min(0, 'Cannot be negative').nullable(true),
      underTarp: Yup.number().integer().min(0, 'Cannot be negative').nullable(true),
    })
    .nullable(true),
});

const houseKeepingPracticeSchema = Yup.object().shape({
  allMaterialsHandled: Yup.object()
    .shape({
      hasPlan: Yup.boolean().nullable(true),
      numSites: Yup.number().integer('Must be an integer').min(0, 'Cannot be negative').nullable(),
    })
    .nullable(true),
  equipmentPreventsOverloading: Yup.object()
    .shape({
      numSites: Yup.number().integer('Must be an integer').min(0, 'Cannot be negative').nullable(),
      hasPlan: Yup.boolean().nullable(true),
    })
    .nullable(true),
  wastewaterSystem: Yup.object()
    .shape({
      numSites: Yup.number().integer('Must be an integer').min(0, 'Cannot be negative').nullable(),
      hasPlan: Yup.boolean().nullable(true),
    })
    .nullable(true),
  controlDiversionExternalWaters: Yup.object()
    .shape({
      numSites: Yup.number().integer('Must be an integer').min(0, 'Cannot be negative').nullable(),
      hasPlan: Yup.boolean().nullable(true),
    })
    .nullable(true),
  drainageCollectionSystem: Yup.object()
    .shape({
      numSites: Yup.number().integer('Must be an integer').min(0, 'Cannot be negative').nullable(),
      hasPlan: Yup.boolean().nullable(true),
    })
    .nullable(true),
  municipalSewerSystem: Yup.object()
    .shape({
      numSites: Yup.number().integer('Must be an integer').min(0, 'Cannot be negative').nullable(),
      hasPlan: Yup.boolean().nullable(true),
    })
    .nullable(true),
  removalContainment: Yup.object()
    .shape({
      numSites: Yup.number().integer('Must be an integer').min(0, 'Cannot be negative').nullable(),
      hasPlan: Yup.boolean().nullable(true),
    })
    .nullable(true),
  watercourse: Yup.object()
    .shape({
      numSites: Yup.number().integer('Must be an integer').min(0, 'Cannot be negative').nullable(),
      hasPlan: Yup.boolean().nullable(true),
    })
    .nullable(true),
  otherDischargePoint: Yup.object()
    .shape({
      numSites: Yup.number().integer('Must be an integer').min(0, 'Cannot be negative').nullable(),
      hasPlan: Yup.boolean().nullable(true),
    })
    .nullable(true),
  ongoingCleanup: Yup.object()
    .shape({
      numSites: Yup.number().integer('Must be an integer').min(0, 'Cannot be negative').nullable(),
      hasPlan: Yup.boolean().nullable(true),
    })
    .nullable(true),
  riskManagementPlan: Yup.object()
    .shape({
      numSites: Yup.number().integer('Must be an integer').min(0, 'Cannot be negative').nullable(),
      hasPlan: Yup.boolean().nullable(true),
    })
    .nullable(true),
});

const vulnerableAreaSchema = Yup.object().shape({
  highwayNumber: Yup.string().max(16).nullable(true),
  latitude: coordinatePrecision(6)
    .min(-90, 'Latitude must be greater than or equal to -90')
    .max(90, 'Latitude must be less than or equal to 90')
    .nullable(true), // aligns with LAT decimal(9,6)
  longitude: coordinatePrecision(6)
    .min(-180, 'Longitude must be greater than or equal to -180')
    .max(180, 'Longitude must be less than or equal to 180')
    .nullable(true), // aligns with LONG decimal(9,6)
  feature: Yup.string().max(255).nullable(true),
  type: Yup.string().max(255).nullable(true),
  protectionMeasures: Yup.string().max(255).nullable(true),
  environmentalMonitoring: Yup.boolean().nullable(true),
  comments: Yup.string().max(255).nullable(true),
});

const typesOfVulnerableAreasSchema = Yup.object().shape({
  drinkingWater: Yup.object()
    .shape({
      areasIdentified: Yup.number().integer().nullable(true),
      areasWithProtection: Yup.number().integer().nullable(true),
      areasWithChloride: Yup.number().integer().nullable(true),
    })
    .nullable(true),
  aquaticLife: Yup.object()
    .shape({
      areasIdentified: Yup.number().integer().nullable(true),
      areasWithProtection: Yup.number().integer().nullable(true),
      areasWithChloride: Yup.number().integer().nullable(true),
    })
    .nullable(true),
  wetlands: Yup.object()
    .shape({
      areasIdentified: Yup.number().integer().nullable(true),
      areasWithProtection: Yup.number().integer().nullable(true),
      areasWithChloride: Yup.number().integer().nullable(true),
    })
    .nullable(true),
  delimitedAreas: Yup.object()
    .shape({
      areasIdentified: Yup.number().integer().nullable(true),
      areasWithProtection: Yup.number().integer().nullable(true),
      areasWithChloride: Yup.number().integer().nullable(true),
    })
    .nullable(true),
  valuedLands: Yup.object()
    .shape({
      areasIdentified: Yup.number().integer().nullable(true),
      areasWithProtection: Yup.number().integer().nullable(true),
      areasWithChloride: Yup.number().integer().nullable(true),
    })
    .nullable(true),
});

const weatherMonitoringSourceSchema = Yup.object().shape({
  relied: Yup.boolean().nullable(true),
  number: Yup.number().integer().nullable(true).min(0, 'Cannot be negative'),
});

export const validationSchema = Yup.object({
  serviceArea: Yup.string().required('Service Area is required'),
  contactName: Yup.string().max(200).required('Contact Name is required'),
  telephone: Yup.string()
    .required('Telephone is required')
    .matches(/^[+]*[(]{0,1}[0-9]{1,4}[)]{0,1}[-\s./0-9]*$/, 'Invalid telephone format'),
  email: Yup.string().max(100).required('Email is required').email('Invalid email format'),
  sect1: Yup.object()
    .shape({
      planDeveloped: Yup.string().max(20).nullable(true),
      planReviewed: Yup.string().max(20).nullable(true),
      planUpdated: Yup.string().max(20).nullable(true),
      training: Yup.object()
        .shape({
          manager: Yup.string().max(20).nullable(true),
          supervisor: Yup.string().max(20).nullable(true),
          operator: Yup.string().max(20).nullable(true),
          mechanical: Yup.string().max(20).nullable(true),
          patroller: Yup.string().max(20).nullable(true),
        })
        .nullable(true),
      objectives: Yup.object()
        .shape({
          materialStorage: Yup.object()
            .shape({
              identified: Yup.number().integer().min(0, 'Cannot be negative').nullable(true),
              achieved: Yup.number().integer().min(0, 'Cannot be negative').nullable(true),
            })
            .nullable(true),
          saltApplication: Yup.object()
            .shape({
              identified: Yup.number().integer().min(0, 'Cannot be negative').nullable(true),
              achieved: Yup.number().integer().min(0, 'Cannot be negative').nullable(true),
            })
            .nullable(true),
        })
        .nullable(true),
    })
    .nullable(true),
  sect2: Yup.object()
    .shape({
      roadTotalLength: Yup.number().integer().nullable(true).min(0, 'Cannot be negative').required('Required'),
      saltTotalDays: Yup.number().integer().nullable(true).min(0, 'Cannot be negative').required('Required'),
    })
    .nullable(true),
  sect3: Yup.object()
    .shape({
      deicer: Yup.object()
        .shape({
          nacl: decimalWithPrecision(2).nullable(true),
          mgcl2: decimalWithPrecision(2).nullable(true),
          cacl2: decimalWithPrecision(2).nullable(true),
          acetate: decimalWithPrecision(2).nullable(true),
        })
        .nullable(true),
      treatedAbrasives: Yup.object()
        .shape({
          sandStoneDust: decimalWithPrecision(2).nullable(true),
          nacl: decimalWithPrecision(2).nullable(true),
          mgcl2: decimalWithPrecision(2).nullable(true),
          cacl2: decimalWithPrecision(2).nullable(true),
        })
        .nullable(true),
      prewetting: Yup.object()
        .shape({
          nacl: decimalWithPrecision(2).nullable(true),
          mgcl2: decimalWithPrecision(2).nullable(true),
          cacl2: decimalWithPrecision(2).nullable(true),
          acetate: decimalWithPrecision(2).nullable(true),
          nonchloride: decimalWithPrecision(2).nullable(true),
        })
        .nullable(true),
      pretreatment: Yup.object()
        .shape({
          nacl: decimalWithPrecision(2).nullable(true),
          mgcl2: decimalWithPrecision(2).nullable(true),
          cacl2: decimalWithPrecision(2).nullable(true),
          acetate: decimalWithPrecision(2).nullable(true),
          nonchloride: decimalWithPrecision(2).nullable(true),
        })
        .nullable(true),
      antiicing: Yup.object()
        .shape({
          nacl: decimalWithPrecision(2).nullable(true),
          mgcl2: decimalWithPrecision(2).nullable(true),
          cacl2: decimalWithPrecision(2).nullable(true),
          acetate: decimalWithPrecision(2).nullable(true),
          nonchloride: decimalWithPrecision(2).nullable(true),
        })
        .nullable(true),
      multiChlorideA: Yup.object()
        .shape({
          litres: decimalWithPrecision(2).nullable(true),
          naclPercentage: decimalWithPrecision(2).nullable(true),
          mgcl2Percentage: decimalWithPrecision(2).nullable(true),
          cacl2Percentage: decimalWithPrecision(2).nullable(true),
        })
        .nullable(true),
      multiChlorideB: Yup.object()
        .shape({
          litres: decimalWithPrecision(2).nullable(true),
          naclPercentage: decimalWithPrecision(2).nullable(true),
          mgcl2Percentage: decimalWithPrecision(2).nullable(true),
          cacl2Percentage: decimalWithPrecision(2).nullable(true),
        })
        .nullable(true),
    })
    .nullable(true),
  sect4: Yup.object()
    .shape({
      saltStorageSitesTotal: Yup.number().integer().min(0).nullable(true),
      stockpiles: Yup.array().of(stockpileSchema).nullable(true),
      practices: houseKeepingPracticeSchema.nullable(true),
    })
    .nullable(true),
  sect5: Yup.object()
    .shape({
      numberOfVehicles: Yup.number().integer().min(0).nullable(true).required('Number of Vehicles is required'),
      vehiclesForSaltApplication: Yup.number()
        .integer()
        .min(0)
        .nullable(true)
        .required('Vehicles for Salt Application is required'),
      vehiclesWithConveyors: Yup.number()
        .integer()
        .min(0)
        .nullable(true)
        .required('Vehicles with Conveyors is required'),
      vehiclesWithPreWettingEquipment: Yup.number()
        .integer()
        .min(0)
        .nullable(true)
        .required('Vehicles with Pre-Wetting Equipment is required'),
      vehiclesForDLA: Yup.number().integer().min(0).nullable(true).required('Vehicles for DLA is required'),
      regularCalibration: Yup.boolean().nullable(true),
      regularCalibrationTotal: Yup.number()
        .nullable(true)
        .when('regularCalibration', {
          is: true,
          then: Yup.number()
            .integer()
            .min(0, 'Frequency must be non-negative')
            .required('Frequency number is required when equipment is regularly calibrated'),
          otherwise: Yup.number().integer().nullable(true),
        }),
      weatherMonitoringSources: Yup.object()
        .shape({
          infraredThermometer: weatherMonitoringSourceSchema,
          meteorologicalService: Yup.object().shape({
            relied: Yup.boolean().nullable(true),
          }),
          fixedRWISStations: weatherMonitoringSourceSchema,
          mobileRWISMounted: weatherMonitoringSourceSchema,
        })
        .nullable(true),
      maintenanceDecisionSupport: Yup.object()
        .shape({
          avl: weatherMonitoringSourceSchema,
          saltApplicationRates: weatherMonitoringSourceSchema,
          applicationRateChart: weatherMonitoringSourceSchema,
          testingMDSS: weatherMonitoringSourceSchema,
        })
        .nullable(true),
    })
    .nullable(true),
  sect6: Yup.object()
    .shape({
      disposal: Yup.object().shape({
        used: Yup.boolean().nullable(true),
        total: Yup.number().integer().min(0).nullable(true),
      }),
      snowMelter: Yup.object().shape({
        used: Yup.boolean().nullable(true),
      }),
      meltwater: Yup.object().shape({
        used: Yup.boolean().nullable(true),
      }),
    })
    .nullable(true),
  sect7: Yup.object()
    .shape({
      completedInventory: Yup.string().nullable(true),
      setVulnerableAreas: Yup.string().nullable(true),
      actionPlanPrepared: Yup.string().nullable(true),
      protectionMeasuresImplemented: Yup.string().nullable(true),
      environmentalMonitoringConducted: Yup.string().nullable(true),
      typesOfVulnerableAreas: typesOfVulnerableAreasSchema.nullable(true),
      vulnerableAreas: Yup.array().of(vulnerableAreaSchema).nullable(true),
    })
    .nullable(true),
});
