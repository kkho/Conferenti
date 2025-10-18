import { dirname } from "path";
import { fileURLToPath } from "url";
import { FlatCompat } from "@eslint/eslintrc";

const __filename = fileURLToPath(import.meta.url);
const __dirname = dirname(__filename);

const compat = new FlatCompat({
  baseDirectory: __dirname,
});

const eslintConfig = [
  ...compat.extends("next/core-web-vitals", "next/typescript"),
  {
    ignores: [
      "node_modules/**",
      ".next/**",
      "out/**",
      "build/**",
      "next-env.d.ts",
    ],
  },
  {
    rules: {
      // Allow debugger statements in development
      "no-debugger": process.env.ENVIRONMENT === "production" ? "error" : "off",
      // Allow console statements in development  
      "no-console": process.env.ENVIRONMENT === "production" ? "warn" : "off",
    },
  },
];

export default eslintConfig;
