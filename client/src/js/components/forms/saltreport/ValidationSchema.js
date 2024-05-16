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

const stockpileSchema = Yup.object().shape({
  siteName: Yup.string(),
  motiOwned: Yup.boolean(),
  roadSalts: Yup.object().shape({
    stockpilesTotal: Yup.number().min(0, 'Cannot be negative').nullable(true),
    onImpermeableSurface: Yup.number().min(0, 'Cannot be negative').nullable(true),
    underPermanentRoof: Yup.number().min(0, 'Cannot be negative').nullable(true),
    underTarp: Yup.number().min(0, 'Cannot be negative').nullable(true),
  }),
  treatedAbrasives: Yup.object().shape({
    stockpilesTotal: Yup.number().min(0, 'Cannot be negative').nullable(true),
    onImpermeableSurface: Yup.number().min(0, 'Cannot be negative').nullable(true),
    underPermanentRoof: Yup.number().min(0, 'Cannot be negative').nullable(true),
    underTarp: Yup.number().min(0, 'Cannot be negative').nullable(true),
  }),
});

const houseKeepingPracticeSchema = Yup.object().shape({
  allMaterialsHandled: Yup.object().shape({
    hasPlan: Yup.boolean(),
    numSites: Yup.number().nullable().min(0, 'Cannot be negative').integer(),
  }),
  equipmentPreventsOverloading: Yup.object().shape({
    numSites: Yup.number().nullable().min(0, 'Cannot be negative').integer(),
    hasPlan: Yup.boolean(),
  }),
  wastewaterSystem: Yup.object().shape({
    numSites: Yup.number().nullable().min(0, 'Cannot be negative').integer(),
    hasPlan: Yup.boolean(),
  }),
  controlDiversionExternalWaters: Yup.object().shape({
    numSites: Yup.number().nullable().min(0, 'Cannot be negative').integer(),
    hasPlan: Yup.boolean(),
  }),
  drainageCollectionSystem: Yup.object().shape({
    numSites: Yup.number().nullable().min(0, 'Cannot be negative').integer(),
    hasPlan: Yup.boolean(),
  }),
  municipalSewerSystem: Yup.object().shape({
    numSites: Yup.number().nullable().min(0, 'Cannot be negative').integer(),
    hasPlan: Yup.boolean(),
  }),
  removalContainment: Yup.object().shape({
    numSites: Yup.number().nullable().min(0, 'Cannot be negative').integer(),
    hasPlan: Yup.boolean(),
  }),
  watercourse: Yup.object().shape({
    numSites: Yup.number().nullable().min(0, 'Cannot be negative').integer(),
    hasPlan: Yup.boolean(),
  }),
  otherDischargePoint: Yup.object().shape({
    numSites: Yup.number().nullable().min(0, 'Cannot be negative').integer(),
    hasPlan: Yup.boolean(),
  }),
  ongoingCleanup: Yup.object().shape({
    numSites: Yup.number().nullable().min(0, 'Cannot be negative').integer(),
    hasPlan: Yup.boolean(),
  }),
  riskManagementPlan: Yup.object().shape({
    numSites: Yup.number().nullable().min(0, 'Cannot be negative').integer(),
    hasPlan: Yup.boolean(),
  }),
});

const vulnerableAreaSchema = Yup.object().shape({
  highway: Yup.string().nullable(),
  latitude: decimalWithPrecision(5),
  longitude: decimalWithPrecision(5),
  feature: Yup.string().nullable(),
  type: Yup.string(),
  protectionMeasures: Yup.string(),
  monitoringInPlace: Yup.boolean(),
});

const typesOfVulnerableAreasSchema = Yup.object().shape({
  drinkingWater: Yup.object().shape({
    areasIdentified: Yup.number().nullable(true),
    areasWithProtection: Yup.number().nullable(true),
    areasWithChloride: Yup.number().nullable(true),
  }),
  aquaticLife: Yup.object().shape({
    areasIdentified: Yup.number().nullable(true),
    areasWithProtection: Yup.number().nullable(true),
    areasWithChloride: Yup.number().nullable(true),
  }),
  wetlands: Yup.object().shape({
    areasIdentified: Yup.number().nullable(true),
    areasWithProtection: Yup.number().nullable(true),
    areasWithChloride: Yup.number().nullable(true),
  }),
  delimitedAreas: Yup.object().shape({
    areasIdentified: Yup.number().nullable(true),
    areasWithProtection: Yup.number().nullable(true),
    areasWithChloride: Yup.number().nullable(true),
  }),
  valuedLands: Yup.object().shape({
    areasIdentified: Yup.number().nullable(true),
    areasWithProtection: Yup.number().nullable(true),
    areasWithChloride: Yup.number().nullable(true),
  }),
});

export const validationSchema = Yup.object({
  serviceArea: Yup.string().required('Service Area is required'),
  contactName: Yup.string().required('Contact Name is required'),
  telephone: Yup.string()
    .required('Telephone is required')
    .matches(/^[+]*[(]{0,1}[0-9]{1,4}[)]{0,1}[-\s./0-9]*$/, 'Invalid telephone format'),
  email: Yup.string().required('Email is required').email('Invalid email format'),
  sect1: Yup.object().shape({
    planDeveloped: Yup.string(),
    planReviewed: Yup.string(),
    planUpdated: Yup.string(),
    training: Yup.object().shape({
      manager: Yup.string(),
      supervisor: Yup.string(),
      operator: Yup.string(),
      mechanical: Yup.string(),
      patroller: Yup.string(),
    }),
    objectives: Yup.object().shape({
      materialStorage: Yup.object().shape({
        identified: Yup.number().min(0, 'Cannot be negative').nullable(),
        achieved: Yup.number().min(0, 'Cannot be negative').nullable(),
      }),
      saltApplication: Yup.object().shape({
        identified: Yup.number().min(0, 'Cannot be negative').nullable(),
        achieved: Yup.number().min(0, 'Cannot be negative').nullable(),
      }),
    }),
  }),
  sect2: Yup.object().shape({
    roadTotalLength: Yup.number().nullable().min(0, 'Cannot be negative').required('Required'),
    saltTotalDays: Yup.number().nullable().min(0, 'Cannot be negative').required(),
  }),
  sect3: Yup.object().shape({
    deicer: Yup.object().shape({
      nacl: decimalWithPrecision(2),
      mgcl2: decimalWithPrecision(2),
      cacl2: decimalWithPrecision(2),
      acetate: decimalWithPrecision(2),
    }),
    treatedAbrasives: Yup.object().shape({
      sandStoneDust: decimalWithPrecision(2),
      nacl: decimalWithPrecision(2),
      mgcl2: decimalWithPrecision(2),
      cacl2: decimalWithPrecision(2),
    }),
    prewetting: Yup.object().shape({
      nacl: decimalWithPrecision(2),
      mgcl2: decimalWithPrecision(2),
      cacl2: decimalWithPrecision(2),
      acetate: decimalWithPrecision(2),
      nonchloride: decimalWithPrecision(2),
    }),
    pretreatment: Yup.object().shape({
      nacl: decimalWithPrecision(2),
      mgcl2: decimalWithPrecision(2),
      cacl2: decimalWithPrecision(2),
      acetate: decimalWithPrecision(2),
      nonchloride: decimalWithPrecision(2),
    }),
    antiicing: Yup.object().shape({
      nacl: decimalWithPrecision(2),
      mgcl2: decimalWithPrecision(2),
      cacl2: decimalWithPrecision(2),
      acetate: decimalWithPrecision(2),
      nonchloride: decimalWithPrecision(2),
    }),
    multiChlorideA: Yup.object().shape({
      litres: decimalWithPrecision(2),
      naclPercentage: decimalWithPrecision(2),
      mgcl2Percentage: decimalWithPrecision(2),
      cacl2Percentage: decimalWithPrecision(2),
    }),
    multiChlorideB: Yup.object().shape({
      litres: decimalWithPrecision(2),
      naclPercentage: decimalWithPrecision(2),
      mgcl2Percentage: decimalWithPrecision(2),
      cacl2Percentage: decimalWithPrecision(2),
    }),
  }),
  sect4: Yup.object()
    .shape({
      saltStorageTotal: Yup.number().nullable(true),
      stockpiles: Yup.array().of(stockpileSchema),
      houseKeepingPractice: houseKeepingPracticeSchema.nullable(true),
    })
    .nullable(true),
  sect5: Yup.object().shape({
    numberOfVehicles: Yup.number().min(0).nullable().required('Number of Vehicles is required'),
    vehiclesForSaltApplication: Yup.number().min(0).nullable().required('Vehicles for Salt Application is required'),
    vehiclesWithConveyors: Yup.number().min(0).nullable().required('Vehicles with Conveyors is required'),
    vehiclesWithPreWettingEquipment: Yup.number()
      .min(0)
      .nullable()
      .required('Vehicles with Pre-Wetting Equipment is required'),
    vehiclesForDLA: Yup.number().min(0).nullable().required('Vehicles for DLA is required'),
    regularCalibration: Yup.boolean().nullable(true),
    regularCalibrationTotal: Yup.number()
      .nullable(true)
      .when('regularCalibration', {
        is: true,
        then: Yup.number()
          .min(0, 'Frequency must be non-negative')
          .required('Frequency number is required when equipment is regularly calibrated'),
        otherwise: Yup.number().nullable(true),
      }),
  }),
  sect7: Yup.object()
    .shape({
      completedInventory: Yup.string(),
      setVulnerableAreas: Yup.string(),
      actionPlanPrepared: Yup.string(),
      protectionMeasuresImplemented: Yup.string(),
      environmentalMonitoringConducted: Yup.string(),
      typesOfVulnerableAreas: typesOfVulnerableAreasSchema,
    })
    .nullable(true),
});
